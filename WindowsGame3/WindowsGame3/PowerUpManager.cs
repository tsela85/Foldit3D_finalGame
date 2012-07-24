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
            foreach (PowerUp p in powerups)
            {
                if (b.PointInBeforeFold(p.getCenter()))
                {
                    if (calcBeforeFolding(foldp1, foldp2, p.getCenter()))
                        p.preFoldData(axis,(foldp1+foldp2)/2);
                    else
                        p.preFoldData(-axis, (foldp1 + foldp2) / 2);
                }
            }
        }
        // I changed it so the axis and the point will be relevent to the closest point - Tom
        public void tomfoldData(float angle, Board b)
        {            
            foreach (PowerUp p in powerups)
            {
                if (b.PointInBeforeFold(p.getCenter()))
                {
                    p.tomfoldData(angle);
                }
            }
        }

        public bool calcBeforeFolding(Vector3 p1, Vector3 p2,Vector3 obj)
        {
            Vector2 foldLine, perpendicular;
            Vector2 first = new Vector2(p1.X, p1.Z);
            Vector2 second = new Vector2(p2.X, p2.Z);
            Vector2 ballRec = new Vector2(obj.X, obj.Z);
            Vector3 center = Vector3.Zero;
            Vector3 refr = new Vector3(-200,0,200);


            foldLine.X = (float)((float)(second.Y - (float)first.Y) / ((float)second.X - (float)first.X));
            perpendicular.X = -1 / foldLine.X;

            if (foldLine.X == 0) //fold line is horizontal
            {
                center.X = ballRec.X;
                center.Z = first.Y;
            }
            else
            {
                if (perpendicular.X == 0) //fold line is vertical
                {
                    center.X = first.X;
                    center.Z = ballRec.Y;
                }
                else
                {
                    foldLine.Y = (float)((float)first.Y - (float)first.X * foldLine.X);
                    perpendicular.Y = ballRec.Y - ballRec.X * perpendicular.X;

                    center.X = -(foldLine.Y - perpendicular.Y) / (foldLine.X - perpendicular.X);
                    center.Z = perpendicular.X * center.X + perpendicular.Y;
                }
            }
            if (Vector3.Distance(refr,center) > Vector3.Distance(refr,obj))
                        return true;
            return false;
        }

        //public void foldData(Vector3 vec, Vector3 point, float angle, Board b)
        //{
        //    foreach (PowerUp p in powerups)
        //    {
        //        if (b.PointInBeforeFold(p.getCenter()))
        //            p.foldData(vec, point, angle);
        //    }
        //}
        #endregion

        #region Collision
        public static void checkCollision(Player player)
        {
            PowerUp pToRemove = null;
            foreach (PowerUp p in powerups)
            {
                BoundingBox b1 = p.getBox();
                b1.Max.X -= 1.0f;
                b1.Max.Z -= 1.0f;
                b1.Min.X += 1.0f;
                b1.Min.Z += 1.0f; 
                BoundingBox b2 = player.getBox();
                if (b1.Intersects(b2))
                {
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
                    return PowerUpType.PlayerSize;
                case 3:
                    return PowerUpType.PlayerPos;
                case 4:
                    return PowerUpType.SplitPlayer;
                case 5:
                    return PowerUpType.DryPlayer;
                case 6:
                    return PowerUpType.NormalPlayer;
                default:
                    return PowerUpType.NormalPlayer;
            }
        }
        #endregion

    }
}
