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

        public StaticPlayer(Texture2D texture, Vector2 c, PlayerManager pm, Effect effect, bool fromDup)
            : base(texture, c, pm, effect, fromDup)
        {
            type = "static";
        }

        #region fold

        public override void foldData(float a, Board.BoardState state)
        {
            if (!beforFold) return;
            if ((a > -MathHelper.Pi + Game1.closeRate) && (moving))
            {
                worldMatrix = Matrix.Identity;
                worldMatrix *= Matrix.CreateTranslation(-point);
                worldMatrix *= Matrix.CreateFromAxisAngle(axis, a);
                worldMatrix *= Matrix.CreateTranslation(point);
            }
        }

        #endregion
    }
}
