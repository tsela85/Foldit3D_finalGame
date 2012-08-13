using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Foldit3D
{
    class Player
    {
        protected bool enabled;
        protected bool moving = true;
        protected bool foldBack = false;
        protected Vector2 worldPosition;
        protected Texture2D texture;
        protected float ROTATION_DEGREE = 0.01f;
        protected Vector2 center = Vector2.Zero;
        protected bool reverse = false;
        protected Color color = Color.White;
        protected int frameWidth;
        protected int frameHeight;
        protected PlayerManager playerManager;
        protected bool isPointsSwitched = false;
        protected VertexPositionTexture[] vertices;
        protected Matrix worldMatrix = Matrix.Identity;
        protected Effect effect;
        protected bool isDraw = true;
        protected bool dataSet = false;
        protected float size = 2f;
        //Tom - added to the
        protected Vector3 axis;
        protected Vector3 point;
        protected bool beforFold,afterFold;
        protected bool stuckOnPaper = false;
        protected int checkCollision = 0;
        // Tom -end


        #region Properties

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }


        public bool Moving
        {
            get { return moving; }
            set { moving = value; }
        }

        public Vector2 WorldPosition
        {
            get { return worldPosition; }
            set { worldPosition = value; }
        }

        public Rectangle WorldRectangle
        {
            get
            {
                return new Rectangle(
                    (int)WorldPosition.X,
                    (int)WorldPosition.Y,
                    frameWidth,
                    frameHeight);
            }
        }

        #endregion

        public Player(Texture2D texture, Vector2 c, PlayerManager pm, Effect effect)
        {
            this.texture = texture;
            frameHeight = texture.Height;
            frameWidth = texture.Width;
            playerManager = pm;
            this.effect = effect;
            //setUpVertices(points);
            center = c;
            setVerts(c);
        }

        #region Update and Draw
        public void Update(GameTime gameTime, GameState state)
        {
            if ((state != GameState.folding) && (checkCollision == -1)) // afterFold
            {

                float oldy;
                stuckOnPaper = true;
               // switchPoints();
                for (int i = 0; i < vertices.Length; i++)
                {
                    oldy = vertices[i].Position.Y;
                    vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
                    vertices[i].Position.Y = oldy;
                }
                calcCenter();
                worldMatrix = Matrix.Identity;

                moving = true;
                dataSet = false;
                HoleManager.checkCollision(this,center, size);
                PowerUpManager.checkCollision(this,center, size);
                checkCollision = 0;
            } else
                if ((state != GameState.folding) && (checkCollision == 1)) // beforeFold
                {
                    dataSet = false;
                    HoleManager.checkCollision(this,center,size);
                    PowerUpManager.checkCollision(this,center, size);
                    checkCollision = 0;
                }
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

                    Game1.device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, VertexPositionTexture.VertexDeclaration);
                }
            }
        }

        #endregion Update and Draw

        #region Fold

        public void preFoldData(Vector3 axis_1, Vector3 point_1, bool beforeF, bool afterF)
        {
            axis = axis_1;
            point = point_1;
            beforFold = beforeF;
            afterFold = afterF;
            stuckOnPaper = false;
        }

        #region Virtual Methods

        //public virtual void foldData(Vector3 axis, Vector3 point, float a,bool beforeFold,bool afterFold) { }
        public virtual void foldData(float a,Board.BoardState state) { }
        #endregion

        #endregion

        #region PowerUps

        //factor - by how much to inlarge (or to make smaller) the player
        //for example:  factor = 2 means that the player will be twice as big, factor = 0.5 half of the size 
        public void changeSize()
        {
            size += 1f;
            setVerts(center);
        }

        public void changePos(){
            setVerts(new Vector2(new Random().Next(-22, 22), new Random().Next(-22, 22)));
        }

        //!!!! i think that posx and posy need to be the postion of the powerup that the player took
        //newtype = normal/static/duplicate
        public void changePlayerType(String newType, int posX, int posY)
        {
            playerManager.changePlayerType(this, newType, posX, posY);
        }
        #endregion

        #region 3D 

        private void setVerts(Vector2 center)
        {
            vertices = new VertexPositionTexture[12];
            Vector3 point1 = new Vector3(center.X - size, -0.0f, center.Y + size);
            Vector3 point2 = new Vector3(center.X + size, -0.0f, center.Y + size);
            Vector3 point3 = new Vector3(center.X + size, -0.0f, center.Y - size);
            Vector3 point4 = new Vector3(center.X - size, -0.0f, center.Y - size);
            Vector3 centerpoint = new Vector3(center.X, -size/2, center.Y);

            vertices[0].Position = point4;
            vertices[0].TextureCoordinate = new Vector2(0.75f, 0);            

            vertices[1].Position = centerpoint;
            vertices[1].TextureCoordinate = new Vector2(0.5f, 0.5f);

            vertices[2].Position = point3;
            vertices[2].TextureCoordinate = new Vector2(1f, 0.85f);


            vertices[3].Position = point2;
            vertices[3].TextureCoordinate = new Vector2(0, 1);

            vertices[4] = vertices[2];
            vertices[5] = vertices[1];

            vertices[6].Position = point1;
            vertices[6].TextureCoordinate = new Vector2(0, 0);

            vertices[7] = vertices[3];
            vertices[8] = vertices[1];

            vertices[9] = vertices[0];
            vertices[10] = vertices[6];
            vertices[11] = vertices[1];            
                                              


           // Trace.WriteLine("\n 0: " + point3 + "\n 1: " + point1 + "\n 2: " + point2 + "\n 3: " + point1 + "\n 4: " + point3 + "\n 5: " + point4);
        }

        public BoundingBox getBox()
        {
            Vector3[] p = new Vector3[2];
            if (isPointsSwitched)
            {
                p[0] = vertices[2].Position;
                p[1] = vertices[5].Position;
            }
            else
            {
                p[0] = vertices[3].Position;
                p[1] = vertices[0].Position;
            }
            return BoundingBox.CreateFromPoints(p);
        }

        public Vector3 getCenter()
        {
            return new Vector3(center.X, 0, center.Y);
        }

        protected void switchPoints()
        {
            VertexPositionTexture temp;
            temp = vertices[0];
            vertices[0] = vertices[2];
            vertices[2] = temp;
            //
            temp = vertices[3];
            vertices[3] = vertices[4];
            vertices[4] = temp;
            //
            temp = vertices[7];
            vertices[7] = vertices[6];
            vertices[6] = temp;
            //
            temp = vertices[9];
            vertices[9] = vertices[10];
            vertices[10] = temp;
            //
            isPointsSwitched = !isPointsSwitched;
        }

        protected void calcCenter()
        {
            center.X = vertices[1].Position.X;
            center.Y = vertices[1].Position.Z;
        }

        #endregion
    }
}
