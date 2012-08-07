using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class PlayerManager
    {
        private Texture2D texture;
        private List<Player> players;
        private Effect effect;
        public PlayerManager(Texture2D texture, Effect effect)
        {
            this.texture = texture;
            players = new List<Player>();
            this.effect = effect;
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
              /*  List<List<Vector3>> lst = new List<List<Vector3>>();
                for(int i =1; i<7; i++){
                    List<Vector3> pointsData = new List<Vector3>();
                    Vector3 point = new Vector3((float)Convert.ToDouble(item["x" + i]), (float)Convert.ToDouble(item["y" + i]), (float)Convert.ToDouble(item["z" + i]));
                    Vector3 texLoc = new Vector3(Convert.ToInt32(item["tX" + i]),Convert.ToInt32(item["tY" + i]),0);
                    pointsData.Add(point);
                    pointsData.Add(texLoc);
                    lst.Add(pointsData);
                }*/
                Vector2 center = new Vector2((float)Convert.ToDouble(item["x"]), (float)Convert.ToDouble(item["y"]));
                players.Add(makeNewPlayer(item["type"], center));
            }
        }

        public void restartLevel()
        {
            players.Clear();
        }
        #endregion

        #region Update and Draw

        public void Draw() {
            foreach (Player p in players)
            {
                p.Draw();
            }
        }
        public void Update(GameTime gameTime, GameState state) {
            foreach (Player p in players)
            {
                p.Update( gameTime, state);
            }
        }

        #endregion

        #region Public Methods

        public Player makeNewPlayer(String type, Vector2 c)
        {
            Player newP = null;
            if (type.CompareTo("normal") == 0)
            {
                newP = new NormalPlayer(texture, c,this, effect);
            }
            else if (type.CompareTo("static") == 0)
            {
                newP = new StaticPlayer(texture, c, this, effect);
            }
            else if (type.CompareTo("duplicate") == 0)
            {
                newP = new DuplicatePlayer(texture, c,this, effect);
            }
            return newP;
        }

       /* public void foldOver()
        {
            foreach (Player p in players)
            {
                p.foldOver();
            }
        }*/

        public void changePlayerType(Player p,String type, int x, int y)
        {
            if (players.Contains(p))
            {
                //players.Add(makeNewPlayer(type, x, y));
                players.Remove(p);
            }
            else Trace.WriteLine("changePlayerType Error!");
        }

        public void preFoldData(Vector3 foldp1, Vector3 foldp2, Vector3 axis, Board b)
        {
            Matrix checkMatrix;
            Vector3 check;
            float rotantionAngle;
            foreach (Player p in players)
            {
                if (b.PointInBeforeFold(p.getCenter()))
                {
                    rotantionAngle = MathHelper.PiOver2;
                    checkMatrix = Matrix.Identity;
                    checkMatrix *= Matrix.CreateFromAxisAngle(axis, rotantionAngle);
                    check = Vector3.Transform(p.getCenter(), checkMatrix); // where the point will be after rotation
                    if (check.Y > 0.0f) // if it is in the right deriction
                        p.preFoldData(axis, (foldp1 + foldp2) / 2, true, false);
                    else // not in the right deriction
                        p.preFoldData(-axis, (foldp1 + foldp2) / 2, true, false);
                }
                else if (b.PointInAfterFold(p.getCenter()))
                {
                    rotantionAngle = -1.5f * MathHelper.PiOver2;
                    checkMatrix = Matrix.Identity;
                    checkMatrix *= Matrix.CreateTranslation(-p.getCenter());
                    checkMatrix *= Matrix.CreateRotationZ(MathHelper.Pi);
                    checkMatrix *= Matrix.CreateTranslation(p.getCenter());
                    checkMatrix *= Matrix.CreateTranslation(-(foldp1 + foldp2) / 2);
                    checkMatrix *= Matrix.CreateFromAxisAngle(axis, rotantionAngle);
                    checkMatrix *= Matrix.CreateTranslation((foldp1 + foldp2) / 2);    
                    check = Vector3.Transform(p.getCenter(), checkMatrix); // where the point will be after rotation
                    if (check.Y > 0.0f) // if it is in the right deriction
                        p.preFoldData(axis, (foldp1 + foldp2) / 2, false, true);
                    else // not in the right deriction
                        p.preFoldData(-axis, (foldp1 + foldp2) / 2, false, true);
                }
            }
        }

        // I changed it so the axis and the point will be relevent to the closest point - Tom
        public void foldData(float angle, Board b)
        {
            foreach (Player p in players)
            {
                if (b.PointInAfterFold(p.getCenter()) || b.PointInBeforeFold(p.getCenter()))
                    p.foldData(angle,b.State);
            }
        }
        #endregion

    }
}
