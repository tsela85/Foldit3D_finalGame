using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class DuplicatePlayer : Player
    {
        private bool created = false;
        public DuplicatePlayer(Texture2D texture, Vector2 c, PlayerManager pm, Effect effect, bool fromDup)
            : base(texture, c, pm, effect, fromDup)
        {
            type = "duplicate";
        }

        #region fold

        public override void foldData(float a, Board.BoardState state)
        {
            if (beforFold)
            {
                if (state == Board.BoardState.folding1 || stuckOnPaper)
                {
                    if ((a > -MathHelper.Pi + Game1.closeRate) && (moving))
                    {
                        worldMatrix = Matrix.Identity;
                        worldMatrix *= Matrix.CreateTranslation(-point);
                        worldMatrix *= Matrix.CreateFromAxisAngle(axis, a);
                        worldMatrix *= Matrix.CreateTranslation(point);
                    }
                }
                else
                    if ((state == Board.BoardState.folding2) && !(stuckOnPaper))
                    {
                      //  float oldy;
                        stuckOnPaper = true;
                        //switchPoints();
                        worldMatrix = Matrix.Identity;
                        worldMatrix *= Matrix.CreateTranslation(-point);
                        worldMatrix *= Matrix.CreateFromAxisAngle(axis, -MathHelper.Pi);
                        worldMatrix *= Matrix.CreateTranslation(point);
                     /*   for (int i = 0; i < vertices.Length; i++)
                        {
                            oldy = vertices[i].Position.Y;
                            vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
                            vertices[i].Position.Y = oldy;
                        }
                        calcCenter();
                        worldMatrix = Matrix.Identity;
                    //    playerManager.makeNewPlayer("normal", center);
                     //  playerManager.changePlayerType(this, "normal", center);*/
                       checkCollision = 1;

                        Vector3 temp = vertices[1].Position;
                        temp = Vector3.Transform(temp, worldMatrix);
                        playerManager.makeNewPlayer("normal", new Vector2(temp.X,temp.Z),true);
                    }
            }
            else if (afterFold && (state == Board.BoardState.folding2))
            {

                worldMatrix = Matrix.Identity;
                worldMatrix *= Matrix.CreateTranslation(-getCenter());
                worldMatrix *= Matrix.CreateRotationZ(MathHelper.Pi);
                worldMatrix *= Matrix.CreateTranslation(getCenter());
                worldMatrix *= Matrix.CreateTranslation(-point);
                worldMatrix *= Matrix.CreateFromAxisAngle(axis, a - MathHelper.Pi);
                worldMatrix *= Matrix.CreateTranslation(point);
                checkCollision = -1;
                if (!stuckOnPaper)
                {
                    stuckOnPaper = true;
                    Vector3 temp = vertices[1].Position;
                    temp = Vector3.Transform(temp, worldMatrix);
                    playerManager.makeNewPlayer("normal", new Vector2(temp.X, temp.Z),true);
                }
            }
        }
        #endregion
    }
}
