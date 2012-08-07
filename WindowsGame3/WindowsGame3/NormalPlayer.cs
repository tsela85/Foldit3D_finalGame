using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Foldit3D
{
    class NormalPlayer : Player
    {       
            

        public NormalPlayer(Texture2D texture, Vector2 c, PlayerManager pm, Effect effect) : base(texture, c, pm, effect) { }

        #region fold
        /*
        public override void foldData(Vector3 axis, Vector3 point, float a, bool beforeFold, bool afterFold)
        {
           // Trace.WriteLine("folding beforeFold " + beforeFold + " afterFold " + afterFold);
            Trace.WriteLine("PLAYER:  axis: " + axis + "   point: " + point + "   a: " + a + "   center: " + center);
            if (!dataSet)
            {
                before = beforeFold;
                after = afterFold;
                if(after) moving = false;
                dataSet = true;
            }

            if (before)
            {
                x = 1;
                z = 1;
                if ((a > -MathHelper.Pi + Game1.closeRate) && (moving))
                {
                   // isDraw = false;
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                    worldMatrix *= Matrix.CreateFromAxisAngle(axis, a);
                    worldMatrix *= Matrix.CreateTranslation(point);   
                  //  checkXZ(axis,point);

               //     worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(x * Math.Abs(axis.X), axis.Y, z * Math.Abs(axis.Z)), -a);
                //    worldMatrix *= Matrix.CreateTranslation(point);
                }
                if (moving && a < -MathHelper.Pi + (2 * Game1.closeRate))
                {
                    moving = false;
                   // isDraw = true;
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                    worldMatrix *= Matrix.CreateFromAxisAngle(axis, a);
                    worldMatrix *= Matrix.CreateTranslation(point);
                //    worldMatrix = Matrix.Identity;
                //    worldMatrix *= Matrix.CreateTranslation(-point);

                //    checkXZ(axis, point);

                   // worldMatrix *= Matrix.CreateFromAxisAngle(axis, -MathHelper.Pi);
                //    worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(x * Math.Abs(axis.X), axis.Y, z * Math.Abs(axis.Z)), -a);
                //    worldMatrix *= Matrix.CreateTranslation(point);
                    switchPoints();
                }
            }

            if (after)
            {
                x = 1; 
                z = 1;

                if (!moving && a < -MathHelper.Pi + (2 * Game1.closeRate))
                {
                    moving = true;
                }

                if (a > -MathHelper.Pi + Game1.closeRate && (moving))
                {
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                    worldMatrix *= Matrix.CreateFromAxisAngle(axis, a);
                    worldMatrix *= Matrix.CreateTranslation(point);   
                  //  isDraw = false;
                  //  worldMatrix = Matrix.Identity;
                   // worldMatrix *= Matrix.CreateTranslation(-point);
                   // worldMatrix *= Matrix.CreateFromAxisAngle(axis, -a);

                   // checkXZ(axis,point);
                    

                   // worldMatrix *= Matrix.CreateTranslation(point);
                }

                if (moving && a > -2*Game1.closeRate)
                {
                    moving = false;
                  //  isDraw = true;
                 //   worldMatrix = Matrix.Identity;
                  //  worldMatrix *= Matrix.CreateTranslation(-point);
                  //  worldMatrix *= Matrix.CreateFromAxisAngle(axis, -MathHelper.Pi);
                    
                  //  checkXZ(axis,point);

                //    worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(x * Math.Abs(axis.X), axis.Y, z * Math.Abs(axis.Z)), -MathHelper.Pi);
                    
                   // worldMatrix *= Matrix.CreateTranslation(point);
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                    worldMatrix *= Matrix.CreateFromAxisAngle(axis, a);
                    worldMatrix *= Matrix.CreateTranslation(point);   
                    switchPoints();
                    
                }
            }
            calcCenter();
        }
*/
        public override void foldData(float a,Board.BoardState state)
        {
            if (beforFold)
            {
                if (state == Board.BoardState.folding1) {
                    if ((a > -MathHelper.Pi + Game1.closeRate) && (moving))
                    {
                        worldMatrix = Matrix.Identity;
                        worldMatrix *= Matrix.CreateTranslation(-point);
                        worldMatrix *= Matrix.CreateFromAxisAngle(axis, a);
                        worldMatrix *= Matrix.CreateTranslation(point);
                    }
                } else
                if ((state == Board.BoardState.folding2) && !(stuckOnPaper))
                {
                    float oldy;
                    stuckOnPaper = true;
                    switchPoints();
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                    worldMatrix *= Matrix.CreateFromAxisAngle(axis, -MathHelper.Pi);
                    worldMatrix *= Matrix.CreateTranslation(point);
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        oldy = vertices[i].Position.Y;
                        vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
                        vertices[i].Position.Y = oldy;
                    }
                    calcCenter();
                    worldMatrix = Matrix.Identity;
                    checkCollision = 1;
                }
            } else if (afterFold && (state == Board.BoardState.folding2))
            {
               //if (!stuckOnPaper)
               //{                   
               //    //turn the gum around
               //    switchPoints();                                   
               //    for (int i = 0; i < vertices.Length; i++)                                          
               //        vertices[i].Position.Y *= -1;
               //    stuckOnPaper = true;
               //}
                worldMatrix = Matrix.Identity;
                worldMatrix *= Matrix.CreateTranslation(-getCenter());
                worldMatrix *= Matrix.CreateRotationZ(MathHelper.Pi);
                worldMatrix *= Matrix.CreateTranslation(getCenter()); 
                worldMatrix *= Matrix.CreateTranslation(-point);                
                worldMatrix *= Matrix.CreateFromAxisAngle(axis,a - MathHelper.Pi);
                worldMatrix *= Matrix.CreateTranslation(point);
                checkCollision = -1;
            }
        }

        #endregion

    }
}
