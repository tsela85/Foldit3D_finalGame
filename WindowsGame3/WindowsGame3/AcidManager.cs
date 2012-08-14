using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class AcidManager
    {
        Texture2D texture;
        private static List<Acid> acids;
        private Effect effect;
        static int collisionCount = 0;

        public AcidManager(Texture2D texture, Effect e)
        {
            this.texture = texture;
            acids = new List<Acid>();
            effect = e;
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
                /* List<List<Vector3>> lst = new List<List<Vector3>>();
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
                acids.Add(new Acid(texture, center, effect));
            }
        }

        public void restartLevel()
        {
            collisionCount = 0;
            acids.Clear();
        }
        #endregion

        #region Draw
        public void setDrawInFold()
        {
            foreach (Acid a in acids)
                a.setDrawInFold();
        }
        public void DrawInFold()
        {
            foreach (Acid a in acids)
                a.DrawInFold();
        }
        public void Draw()
        {
            foreach (Acid a in acids)
                a.Draw();
        }
        #endregion

        #region Update
        public void Update(GameState state)
        {
            foreach (Acid a in acids)
                a.Update(state);
        }
        #endregion

        #region Public Methods

        public void preFoldData(Vector3 foldp1, Vector3 foldp2, Vector3 axis, Board b)
        {
            Matrix checkMatrix;
            Vector3 check;
            foreach (Acid a in acids)
            {
                if (b.PointInBeforeFold(a.getCenter()))
                {
                    checkMatrix = Matrix.Identity;
                    checkMatrix *= Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(90));
                    check = Vector3.Transform(a.getCenter(), checkMatrix); // where the point will be after rotation
                    if (check.Y > 0.0f) // if it is in the right deriction
                        a.preFoldData(axis, (foldp1 + foldp2) / 2);
                    else // not in the right deriction
                        a.preFoldData(-axis, (foldp1 + foldp2) / 2);
                }
            }
        }
        // I changed it so the axis and the point will be relevent to the closest point - Tom
        public void foldData(float angle, Board b)
        {
            foreach (Acid a in acids)
            {
                if (b.PointInBeforeFold(a.getCenter()))
                {
                    a.foldData(angle);
                }
            }
        }


        #endregion

        #region Collision
        public static bool isStillInacid(Player player, Vector2 pCenter, float pSize)
        {
            foreach (Acid a in acids)
            {
                if (Vector2.Distance(pCenter, a.center) < (pSize + a.size - 0.5))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool checkCollision(Player player, Vector2 pCenter, float pSize)
        {
            foreach (Acid a in acids)
            {
                if (Vector2.Distance(pCenter, a.center) < (pSize + a.size - 0.5))
                {
                    Trace.WriteLine("Lose!!!!!!");
                    GameManager.loseLevel();
                    return true;
                }
            }
            return false;
        }

        public static bool collideWithAcids(Vector2 loc)
        {
            foreach (Acid a in AcidManager.acids)
            {
                if (Vector2.Distance(loc, a.center) < (a.size + a.size)) return true;
            }

            return false;
        }
        #endregion
    }
}
