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
        public DuplicatePlayer(Texture2D texture, Vector2 c, PlayerManager pm, Effect effect) : base(texture, c, pm, effect) { }

        #region fold

      /*  public override void foldData(Vector3 axis, Vector3 point, float a)
        {
            worldMatrix = Matrix.Identity;
            float angle = MathHelper.ToDegrees(a);
            if (angle < 167 && angle >= 0)
            {
                worldMatrix *= Matrix.CreateTranslation(-point);
                worldMatrix *= Matrix.CreateFromAxisAngle(axis, a);
                worldMatrix *= Matrix.CreateTranslation(point);
            }
            else if (angle > 167 && !created)
            {
                moving = false;
                created = true;
               // playerManager.makeNewPlayer("normal", (int)worldPosition.X, (int)worldPosition.Y);
            }
        }
        */
        #endregion
    }
}
