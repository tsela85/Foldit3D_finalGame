using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Foldit3D;

namespace WindowsGame3
{
    class Table
    {
        // the table that all of the objects are drawn on, and table model's 
        // absoluteBoneTransforms. Since the table is not animated, these can be 
        // calculated once and saved.
         Model table;
        static Matrix[] tableAbsoluteBoneTransforms;

        public  Table(Model table3d)
        {
            // now that we've loaded in the models that will sit on the table, go
            // through the same procedure for the table itself.            
            table = table3d;
            tableAbsoluteBoneTransforms = new Matrix[table.Bones.Count];
            table.CopyAbsoluteBoneTransformsTo(tableAbsoluteBoneTransforms);
        }

             /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        public void Draw()
        {
            Matrix world = Matrix.Identity;
           // world.Up = world.Down;
            world *= Matrix.CreateScale(50);
            world *= Matrix.CreateRotationZ(MathHelper.Pi);
            world *= Matrix.CreateTranslation(new Vector3(0, 1, 0));
            // Draw the table.
            DrawModel(table, world, tableAbsoluteBoneTransforms);
        }

        /// <summary>
        /// DrawModel is a helper function that takes a model, world matrix, and
        /// bone transforms. It does just what its name implies, and draws the model.
        /// </summary>
        private void DrawModel(Model model, Matrix worldTransform,
                                            Matrix[] absoluteBoneTransforms)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.View = Game1.camera.View;
                    effect.Projection = Game1.camera.Projection;
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] *
                                                                        worldTransform;
                }

                mesh.Draw();
            }
        }

    }
}
