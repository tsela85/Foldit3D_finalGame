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
        private Texture2D dupTexture;
        private Texture2D staticTexture;
        private List<Player> players;
        private Effect effect;
        public PlayerManager(Texture2D texture, Effect effect, Texture2D texture1, Texture2D texture2)
        {
            this.texture = texture;
            players = new List<Player>();
            this.effect = effect;
            dupTexture = texture1;
            staticTexture = texture2;
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
                Vector2 center = new Vector2((float)Convert.ToDouble(item["x"]), (float)Convert.ToDouble(item["y"]));
                makeNewPlayer(item["type"], center);
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
           // foreach (Player p in players)
            int i = 0;
            while (i<players.Count())
            {
                players.ElementAt(i).Update( gameTime, state);
                i++;
            }
        }

        #endregion

        #region Public Methods

        public void changeAllStatic()
        {
             int i = 0;
             while (i < players.Count())
             {
                 if (players.ElementAt(i).type.CompareTo("static") == 0)
                 {
                     Vector3 cent = players.ElementAt(i).getCenter();
                     changePlayerType(players.ElementAt(i), "normal", new Vector2(cent.X, cent.Z));
                 }
                 i++;
             }
        }

        public Player makeNewPlayer(String type, Vector2 c)
        {
            Player newP = null;
            if (type.CompareTo("normal") == 0)
            {
                players.Add(new NormalPlayer(texture, c,this, effect));
            }
            else if (type.CompareTo("static") == 0)
            {
                players.Add(new StaticPlayer(staticTexture, c, this, effect));
            }
            else if (type.CompareTo("duplicate") == 0)
            {
                players.Add(new DuplicatePlayer(dupTexture, c,this, effect));
            }
            return newP;
        }

        public void changePlayerType(Player p,String type, Vector2 center)
        {
            if (players.Contains(p))
            {
                makeNewPlayer(type,center);
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
            int i = 0;
            while (i<players.Count())
            {
                if (b.PointInAfterFold(players.ElementAt(i).getCenter()) || b.PointInBeforeFold(players.ElementAt(i).getCenter()))
                    players.ElementAt(i).foldData(angle, b.State);
                i++;

            }
        }

        public int getNumOfPlayers()
        {
            return players.Count();
        }
        #endregion

    }
}
