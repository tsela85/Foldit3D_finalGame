﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class HoleManager
    {
        Texture2D texture;
        private static List<Hole> holes;
        private Effect effect;
        static int collisionCount = 0;

        public HoleManager(Texture2D texture, Effect e)
        {
            this.texture = texture;
            holes = new List<Hole>();
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
                holes.Add(new Hole(texture, center, effect));
            }
        }

        public void restartLevel()
        {
            collisionCount = 0;
            holes.Clear();
        }
        #endregion

        #region Draw
        public void setDrawInFold()
        {
            foreach (Hole h in holes)
                h.setDrawInFold();
        }
        public void DrawInFold()
        {
            foreach (Hole h in holes)
                h.DrawInFold();
        }
        public void Draw()
        {
            foreach (Hole hole in holes)
                hole.Draw();
        }
        #endregion

        #region Update
        public void Update(GameState state)
        {
            foreach (Hole h in holes)
                h.Update(state);
        }
        #endregion

        #region Public Methods

        public void preFoldData(Vector3 foldp1, Vector3 foldp2, Vector3 axis, Board b)
        {
            Matrix checkMatrix;
            Vector3 check;
            foreach (Hole h in holes)
            {
                if (b.PointInBeforeFold(h.getCenter()))
                {
                    checkMatrix = Matrix.Identity;
                    checkMatrix *= Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(90));
                    check = Vector3.Transform(h.getCenter(), checkMatrix); // where the point will be after rotation
                    if (check.Y > 0.0f) // if it is in the right deriction
                        h.preFoldData(axis, (foldp1 + foldp2) / 2);
                    else // not in the right deriction
                        h.preFoldData(-axis, (foldp1 + foldp2) / 2);
                }
            }
        }
        // I changed it so the axis and the point will be relevent to the closest point - Tom
        public void foldData(float angle, Board b)
        {
            foreach (Hole h in holes)
            {
                if (b.PointInBeforeFold(h.getCenter()))
                {
                    h.foldData(angle);
                }
            }
        }

  
        #endregion

        #region Collision
        public static bool isStillInHole(Player player,Vector2 pCenter,float pSize)
        {
            foreach (Hole h in holes)
            {
                if (Vector2.Distance(pCenter, h.center) < (pSize + h.size - 0.5))
                {
                    return true;
                }
            }

            return false;
        }

        public static void checkCollisionLevel3(bool isWin)
        {
            if (isWin)
            {
                // WIN!!!
                Trace.WriteLine("WIN!!!!!!");
                GameManager.winLevel();
            }
        }
        public static bool checkCollision(Player player,Vector2 pCenter,float pSize, int numOfPlayers)
        {
            foreach (Hole h in holes)
            {
                if (Vector2.Distance(pCenter, h.center) < (pSize + h.size - 0.5))
                {
                    GameManager.showHoleMsg = true;
                    if (GameManager.level != 3)
                    {
                        collisionCount++;
                        if (collisionCount >= numOfPlayers)
                        {
                            // WIN!!!
                            Trace.WriteLine("WIN!!!!!!");
                            GameManager.winLevel();
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool collideWithHoles(Vector2 loc)
        {
            foreach (Hole h in HoleManager.holes)
            {
                if (Vector2.Distance(loc, h.center) < (h.size + h.size)) return true;
            }

            return false;
        }
        #endregion

        #region ChangeHoles
        public static void changeAllHolesPlace()
        {
            int pos = new Random().Next(holes.Count);
            holes[pos].changePos();
        }
        public static void cangeAllHolesSize()
        {
            int pos = new Random().Next(holes.Count);
            holes[pos].changeSize();
        }
        #endregion
    }
}
