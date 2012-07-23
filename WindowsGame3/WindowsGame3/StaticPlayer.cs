using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class StaticPlayer : Player
    {

        public StaticPlayer(Texture2D texture, Vector2 c, PlayerManager pm, Effect effect) : base(texture, c, pm, effect) { }

        #region fold

       /* public override void foldData(Vector3 axis, Vector3 point, float a)
        {
            worldMatrix = Matrix.Identity;
            float angle = MathHelper.ToDegrees(a);
            if (a > -MathHelper.Pi + Game1.closeRate)
            {
                worldMatrix *= Matrix.CreateTranslation(-point);
                worldMatrix *= Matrix.CreateFromAxisAngle(axis, -a);
                worldMatrix *= Matrix.CreateTranslation(point);
            }
        }*/

        #endregion
    }
}
