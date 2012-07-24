using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Foldit3D
{
    enum PowerUpType { HoleSize, HolePos, PlayerSize, PlayerPos, SplitPlayer, DryPlayer, NormalPlayer };

    class PowerUp
    {
        Vector2 center = Vector2.Zero;
        bool moving = true;
        bool isDraw = true;
        bool drawInFold = false;
        float size = 2.5f;
        Texture2D texture;
        Vector2 worldPosition;
        PowerUpType type;

        protected VertexPositionTexture[] vertices;
        protected Matrix worldMatrix = Matrix.Identity;
        protected Effect effect;

        public PowerUp(Texture2D t, PowerUpType ty, Vector2 c, Effect e) 
        {
            texture = t;
            type = ty;
            effect = e;
            center = c;
            setVerts(c);
        }

        #region Properties
        public Vector2 WorldPosition
        {
            get { return worldPosition; }
            set { worldPosition = value; }
        }

        #endregion

        #region Action
        public void doYourThing(Player player)
        {
            switch (type)
            {
                // V
                case PowerUpType.HoleSize:
                    HoleManager.cangeAllHolesSize();
                    break;
                // V
                case PowerUpType.HolePos:
                    HoleManager.changeAllHolesPlace();
                    break;
                // V
                case PowerUpType.PlayerSize:
                    player.changeSize();
                    break;
                // V
                case PowerUpType.PlayerPos:
                    player.changePos();
                    break;
                case PowerUpType.SplitPlayer:
                    player.changePlayerType("duplicate", (int)worldPosition.X, (int)worldPosition.Y);
                    break;
                case PowerUpType.DryPlayer:
                    player.changePlayerType("static", (int)worldPosition.X, (int)worldPosition.Y);
                    break;
                case PowerUpType.NormalPlayer:
                    player.changePlayerType("normal", (int)worldPosition.X, (int)worldPosition.Y);
                    break;
            }
        }
        #endregion

        #region Update
        public void Update(GameState state)
        {
            //if (state == GameState.folding && dataWasCalced)
            //    rotate();
            if (state != GameState.folding)
            {
                Trace.WriteLine(state);
                moving = true;
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
            }
        }
        #endregion

        #region fold


        public void foldData(Vector3 axis, Vector3 point, float a)
        {
            //int x = 1 , z = 1;

            if ((a > -MathHelper.Pi + Game1.closeRate) && (moving))
            {
                worldMatrix = Matrix.Identity;
                worldMatrix *= Matrix.CreateTranslation(-point);
                worldMatrix *= Matrix.CreateFromAxisAngle(axis, a);
                worldMatrix *= Matrix.CreateTranslation(point);   
               // Trace.WriteLine("POWERUP: axis: " + axis + "   point: " + point + "   a: " + a + "   center: "+center);
              //  if (angle < -90) isDraw = false;
              //  else isDraw = true;
              //  worldMatrix = Matrix.Identity;
                //worldMatrix *= Matrix.CreateTranslation(-point);
                // worldMatrix *= Matrix.CreateFromAxisAngle(axis, -a);

                #region change x and z
                // powerup: right+down
                /*     if (center.X > 0 && center.Y < 0 && axis.X > 0 && axis.Z > 0) { x = -1; z = -1; }
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
                     if (center.X < 0 && center.Y > 0 && axis.X < 0 && axis.Z > 0 && point.X < 0 && point.Z > 0) { x = -1; z = 1; }
                     if (center.X < 0 && center.Y > 0 && axis.X < 0 && axis.Z < 0 && point.X > 0 && point.Z < 0) { x = -1; z = 1; }
                     if (center.X < 0 && center.Y > 0 && axis.X > 0 && axis.Z > 0) { x = 1; z = 1; }
                     if (center.X < 0 && center.Y > 0 && axis.X > 0 && axis.Z < 0 && point.X > 0 && point.Z > 0) { x = 1; z = -1; } 
                     if (center.X < 0 && center.Y > 0 && axis.X > 0 && axis.Z < 0 && point.X > 0 && point.Z < 0) { x = -1; z = 1; }
               
               
                     //if (center.X < 0 && center.Y > 0 && axis.X < 0 && axis.Z < 0 && point.X < 0 && point.Z < 0) { x = -1; z = 1; }
                */
                #endregion

       //         worldMatrix *= Matrix.CreateFromAxisAngle(new Vector3(x * Math.Abs(axis.X), axis.Y, z * Math.Abs(axis.Z)), -a);
               
           //     worldMatrix *= Matrix.CreateTranslation(point);
            }
        }

        #endregion

        #region Draw
        public void setDrawInFold()
        {
            //drawInFold = false;

            //XXX - for debug
            drawInFold = true;
        }
        public void DrawInFold()
        {
            if (drawInFold)
                Draw();
        }

        public void Draw()
        {
            if (isDraw)
            {
                effect.CurrentTechnique = effect.Techniques["TexturedNoShading"];
                effect.Parameters["xWorld"].SetValue(worldMatrix);
                effect.Parameters["xView"].SetValue(Game1.camera.View);
                effect.Parameters["xProjection"].SetValue(Game1.camera.Projection);
                effect.Parameters["xTexture"].SetValue(texture);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    Game1.device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2, VertexPositionTexture.VertexDeclaration);
                }
            }
        }
        #endregion

        #region 3D

        private void setVerts(Vector2 center)
        {
            vertices = new VertexPositionTexture[6];
            Vector3 point1 = new Vector3(center.X - size, 0, center.Y + size);
            Vector3 point2 = new Vector3(center.X + size, 0, center.Y + size);
            Vector3 point3 = new Vector3(center.X + size, 0, center.Y - size);
            Vector3 point4 = new Vector3(center.X - size, 0, center.Y - size);

            vertices[0].Position = point3;
            vertices[0].TextureCoordinate = new Vector2(1,1);

            vertices[1].Position = point1;
            vertices[1].TextureCoordinate = new Vector2(0, 0);

            vertices[2].Position = point2;
            vertices[2].TextureCoordinate = new Vector2(1, 0);

            vertices[3] = vertices[1];
            vertices[4] = vertices[0];

            vertices[5].Position = point4;
            vertices[5].TextureCoordinate = new Vector2(0, 1);
        }

        public BoundingBox getBox()
        {
            Vector3[] p = new Vector3[2];
            p[0] = vertices[2].Position;
            p[1] = vertices[5].Position;
            return BoundingBox.CreateFromPoints(p);
        }

        public Vector3 getCenter()
        {
            return new Vector3(center.X, 0, center.Y);
        }
        #endregion
    }
}
