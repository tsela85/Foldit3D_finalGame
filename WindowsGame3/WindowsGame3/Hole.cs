﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Foldit3D
{
    class Hole
    {
        Vector2 center = Vector2.Zero;
        bool moving = true;
        bool isDraw = true;
        bool drawInFold = false;
        float size = 3f;
        Texture2D texture;
        Vector2 worldPosition;
        //Tom - added to the
        Vector3 axis;
        Vector3 point;
        // Tom -end



        protected VertexPositionTexture[] vertices;
        protected Matrix worldMatrix = Matrix.Identity;
        protected Effect effect;

        public Hole(Texture2D texture, Vector2 c, Effect e)
        {
            this.texture = texture;
           // worldRectangle = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, texture.Width, texture.Height);
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

        #region Update
        public void Update(GameState state)
        {
           // if (state == GameState.folding && dataWasCalced)
           //     rotate();
            if (state != GameState.folding)
            {
                Trace.WriteLine(state);
                moving = true;
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
            }
        }
        #endregion

        #region Fold

        public void preFoldData(Vector3 axis_1, Vector3 point_1)
        {
            axis = axis_1;
            point = point_1;
        }

        public void foldData(float a)
        {
            if ((a > -MathHelper.Pi + Game1.closeRate) && (moving))
            {
                worldMatrix = Matrix.Identity;
                worldMatrix *= Matrix.CreateTranslation(-point);
                worldMatrix *= Matrix.CreateFromAxisAngle(axis, a);
                worldMatrix *= Matrix.CreateTranslation(point);
            }
        }

         
        #endregion

        #region Public Methods
        public void changePos()
        {
            Vector2 v = new Vector2(new Random().Next(-22, 22), new Random().Next(-22, 22));
            center = v;
            setVerts(v);
        }

        public void changeSize()
        {
            size += 1f;
            setVerts(center);
        }
        #endregion

        #region 3D

        private void setVerts(Vector2 center)
        {
            vertices = new VertexPositionTexture[6];
            Vector3 point1 = new Vector3(center.X - size, -0.01f, center.Y + size);
            Vector3 point2 = new Vector3(center.X + size, -0.01f, center.Y + size);
            Vector3 point3 = new Vector3(center.X + size, -0.01f, center.Y - size);
            Vector3 point4 = new Vector3(center.X - size, -0.01f, center.Y - size);

            vertices[0].Position = point3;
            vertices[0].TextureCoordinate = new Vector2(1, 1);

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
