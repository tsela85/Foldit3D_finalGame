using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class PowerUpManager
    {
        Texture2D texture;
        private static List<PowerUp> powerups;
        private Effect effect;

        public PowerUpManager(Texture2D texture, Effect e)
        {
            this.texture = texture;
            powerups = new List<PowerUp>();
            effect = e;
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
                /*List<List<Vector3>> lst = new List<List<Vector3>>();
                for (int i = 1; i < 7; i++)
                {
                    List<Vector3> pointsData = new List<Vector3>();
                    Vector3 point = new Vector3((float)Convert.ToDouble(item["x" + i]), (float)Convert.ToDouble(item["y" + i]), (float)Convert.ToDouble(item["z" + i]));
                    Vector3 texLoc = new Vector3(Convert.ToInt32(item["tX" + i]), Convert.ToInt32(item["tY" + i]), 0);
                    pointsData.Add(point);
                    pointsData.Add(texLoc);
                    lst.Add(pointsData);
                }*/

                Vector2 center = new Vector2((float)Convert.ToDouble(item["x"]), (float)Convert.ToDouble(item["y"]));
                powerups.Add(new PowerUp(texture, ConvertType(Convert.ToInt32(item["type"])), center, effect));
            }
        }

        public void tomInitLevel()
        {
            for (int i = -37; i < 40; i+=10)
                for(int j = -27; j < 30; j += 10)
                    powerups.Add(new PowerUp(texture, 0, new Vector2(i,j), effect));
        }

        public void restartLevel()
        {
            powerups.Clear();
        }
        #endregion

        #region Draw
        public void setDrawInFold()
        {
            foreach (PowerUp p in powerups)
                p.setDrawInFold();
        }
        public void DrawInFold()
        {
            foreach (PowerUp p in powerups)
                p.DrawInFold();
        }
        public void Draw()
        {
            foreach (PowerUp p in powerups)
                p.Draw();
        }
        #endregion

        #region Update
        public void Update(GameState state)
        {
            foreach (PowerUp p in powerups)
                p.Update(state);
        }
        #endregion

        #region Public Methods

        public void preFoldData(Vector3 foldp1, Vector3 foldp2, Vector3 axis, Board b)
        {
            Matrix checkMatrix;
            Vector3 check;
            foreach (PowerUp p in powerups)
            {
                if (b.PointInBeforeFold(p.getCenter()))
                {
                    checkMatrix = Matrix.Identity;
                    checkMatrix *= Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(90));
                    check = Vector3.Transform(p.getCenter(), checkMatrix); // where the point will be after rotation
                    if (check.Y > 0.0f) // if it is in the right deriction
                        p.preFoldData(axis,(foldp1+foldp2)/2);
                    else // not in the right deriction
                        p.preFoldData(-axis, (foldp1 + foldp2) / 2);                
                }
            }
        }
        // I changed it so the axis and the point will be relevent to the closest point - Tom
        public void foldData(float angle, Board b)
        {            
            foreach (PowerUp p in powerups)
            {
                if (b.PointInBeforeFold(p.getCenter()))
                {
                    p.foldData(angle);
                }
            }
        }

        #endregion

        #region Collision
        public static void checkCollision(Player player, Vector2 pCenter, float pSize)
        {
            PowerUp pToRemove = null;
            foreach (PowerUp p in powerups)
            {
                if (Vector2.Distance(pCenter, p.center) < (pSize + p.size))
                {
                    GameManager.showPuMsg = true;
                    p.doYourThing(player);
                    pToRemove = p;
                    break;
                }
            }
            if (pToRemove!=null)
                powerups.Remove(pToRemove);
        }
        #endregion

        #region Private Methods
        private PowerUpType ConvertType(int type)
        {
            switch (type)
            {
                case 0:
                    return PowerUpType.HoleSize;
                case 1:
                    return PowerUpType.HolePos;
                case 2:
                    return PowerUpType.BiggerPlayerSize;
                case 3:
                    return PowerUpType.SmallerPlayerSize;
                case 4:
                    return PowerUpType.SplitPlayer;
                case 5:
                    return PowerUpType.NormalPlayer;
                case 6:
                    return PowerUpType.ChangeFolds;
                case 7:
                    return PowerUpType.ExtraTime;
                default:
                    return PowerUpType.NormalPlayer;
            }
        }
        #endregion

    }
}
