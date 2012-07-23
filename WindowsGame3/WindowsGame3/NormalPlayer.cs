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
        private bool before = false;
        private bool after = false;
        int x = 1, z = 1;

        public NormalPlayer(Texture2D texture, Vector2 c, PlayerManager pm, Effect effect) : base(texture, c, pm, effect) { }

        #region fold

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
                  //  worldMatrix *= Matrix.CreateFromAxisAngle(axis, -a);

                    checkXZ(axis,point);

                    worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(x * Math.Abs(axis.X), axis.Y, z * Math.Abs(axis.Z)), -a);
                    worldMatrix *= Matrix.CreateTranslation(point);
                }
                if (moving && a < -MathHelper.Pi + (2 * Game1.closeRate))
                {
                    moving = false;
                   // isDraw = true;
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);

                    checkXZ(axis, point);

                   // worldMatrix *= Matrix.CreateFromAxisAngle(axis, -MathHelper.Pi);
                    worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(x * Math.Abs(axis.X), axis.Y, z * Math.Abs(axis.Z)), -a);
                    worldMatrix *= Matrix.CreateTranslation(point);
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
                  //  isDraw = false;
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                   // worldMatrix *= Matrix.CreateFromAxisAngle(axis, -a);

                    checkXZ(axis,point);
                    

                    worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(x * Math.Abs(axis.X), axis.Y, z * Math.Abs(axis.Z)), -a); 
                    worldMatrix *= Matrix.CreateTranslation(point);
                }

                if (moving && a > -2*Game1.closeRate)
                {
                    moving = false;
                  //  isDraw = true;
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                  //  worldMatrix *= Matrix.CreateFromAxisAngle(axis, -MathHelper.Pi);
                    
                    checkXZ(axis,point);

                    worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(x * Math.Abs(axis.X), axis.Y, z * Math.Abs(axis.Z)), -MathHelper.Pi);
                    
                    worldMatrix *= Matrix.CreateTranslation(point);
                    switchPoints();
                    
                }
            }
            calcCenter();
        }


        public override void switchPoints()
        {
            VertexPositionTexture temp;
            temp = vertices[0];
            vertices[0] = vertices[1];
            vertices[1] = temp;
            temp = vertices[3];
            vertices[3] = vertices[4];
            vertices[4] = temp;

            isPointsSwitched = !isPointsSwitched;
        }

        public void checkXZ(Vector3 axis, Vector3 point)
        {
            #region change x and z
            // powerup: right+down
            if (center.X > 0 && center.Y < 0 && axis.X > 0 && axis.Z > 0) { x = -1; z = -1; }
            if (center.X > 0 && center.Y < 0 && axis.X > 0 && axis.Z > 0 && point.X > 0 && point.Z < 0) { x = 1; z = 1; }
            if (center.X > 0 && center.Y < 0 && axis.X > 0 && axis.Z < 0 && point.X > 0 && point.Z < 0) { x = 1; z = -1; }
            if (center.X > 0 && center.Y < 0 && axis.X > 0 && axis.Z < 0 && point.X < 0 && point.Z > 0) { x = -1; z = 1; }
            if (center.X > 0 && center.Y < 0 && axis.X < 0 && axis.Z > 0 && point.X > 0 && point.Z > 0) { x = 1; z = -1; }
            if (center.X > 0 && center.Y < 0 && axis.X < 0 && axis.Z > 0 && point.X < 0 && point.Z < 0) { x = -1; z = 1; }
            if (center.X > 0 && center.Y < 0 && axis.X < 0 && axis.Z > 0 && point.X < 0 && point.Z > 0) { x = 1; z = -1; }
            if (center.X > 0 && center.Y < 0 && axis.X < 0 && axis.Z < 0) { x = -1; z = 1; }
            if (center.X > 0 && center.Y < 0 && axis.X < 0 && axis.Z < 0 && point.X > 0 && point.Z < 0) { x = -1; z = -1; }
            if (center.X > 0 && center.Y < 0 && axis.X < 0 && axis.Z < 0 && point.X < 0 && point.Z < 0) { x = -1; z = -1; }

            // powerup: right+up
            if (center.X > 0 && center.Y > 0 && axis.X > 0 && axis.Z > 0) { x = -1; z = -1; }
            if (center.X > 0 && center.Y > 0 && axis.X > 0 && axis.Z > 0 && point.X > 0 && point.Z > 0) { x = -1; z = -1; } //
            if (center.X > 0 && center.Y > 0 && axis.X > 0 && axis.Z < 0 && point.X > 0 && point.Z < 0) { x = 1; z = -1; }
            if (center.X > 0 && center.Y > 0 && axis.X > 0 && axis.Z < 0 && point.X > 0 && point.Z > 0) { x = 1; z = 1; } //
            if (center.X > 0 && center.Y > 0 && axis.X < 0 && axis.Z > 0) { x = 1; z = -1; }
            if (center.X > 0 && center.Y > 0 && axis.X < 0 && axis.Z < 0 && point.X < 0 && point.Z < 0) { x = -1; z = -1; }
            if (center.X > 0 && center.Y > 0 && axis.X < 0 && axis.Z < 0 && point.X > 0 && point.Z < 0) { x = -1; z = -1; }

            // powerup: left+down
            if (center.X < 0 && center.Y < 0 && axis.X > 0 && axis.Z > 0) { x = 1; z = 1; }
            if (center.X < 0 && center.Y < 0 && axis.X > 0 && axis.Z < 0 && point.X < 0 && point.Z < 0) { x = -1; z = 1; }
            if (center.X < 0 && center.Y < 0 && axis.X > 0 && axis.Z < 0 && point.X > 0 && point.Z < 0) { x = -1; z = 1; }
            if (center.X < 0 && center.Y < 0 && axis.X < 0 && axis.Z > 0) { x = -1; z = 1; }
            if (center.X < 0 && center.Y < 0 && axis.X < 0 && axis.Z < 0 && point.X > 0 && point.Z < 0) { x = 1; z = 1; }

            // powerup: left+up
            if (center.X < 0 && center.Y > 0 && axis.X < 0 && axis.Z > 0) { x = -1; z = 1; }
            if (center.X < 0 && center.Y > 0 && axis.X < 0 && axis.Z > 0 && point.X < 0 && point.Z > 0) { x = 1; z = -1; }
            if (center.X < 0 && center.Y > 0 && axis.X > 0 && axis.Z > 0) { x = 1; z = 1; }
            if (center.X < 0 && center.Y > 0 && axis.X > 0 && axis.Z < 0 && point.X > 0 && point.Z < 0) { x = 1; z = -1; }
            if (center.X < 0 && center.Y > 0 && axis.X > 0 && axis.Z < 0 && point.X > 0 && point.Z > 0) { x = 1; z = -1; }
            if (center.X < 0 && center.Y > 0 && axis.X < 0 && axis.Z < 0 && point.X > 0 && point.Z < 0) { x = -1; z = 1; }

            #endregion
        }

        public void calcCenter()
        {
            center.X = Vector3.Transform(vertices[2].Position, worldMatrix).X - size;
            center.Y = Vector3.Transform(vertices[2].Position, worldMatrix).Z - size;
        }

        #endregion

    }
}
