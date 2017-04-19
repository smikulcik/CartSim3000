using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingSpree
{
    class Mirror : GameObject
    {
        public Camera mirrorCam;

        public Mirror(Vector3 position, Quaternion quat, float scale, GraphicsDevice device) :
            base(position, quat, scale, null)
        {
            mirrorCam = new Camera(position, 0, device);
        }

        public void Update(GameTime gameTime, Camera fromWhom)
        {
            Vector3 mirrorNormal = Vector3.Transform(Vector3.Forward, Rotation);

            //get components of fromWhom's position in relation to mirror
            Vector3 mirrorToFromWhomPerp = Vector3.Dot(fromWhom.Pos - Pos, mirrorNormal) * mirrorNormal;
            Vector3 mirrorToFromWhomPara = (fromWhom.Pos - Pos) - mirrorToFromWhomPerp;

            //invert the perpendicular component
            mirrorCam.Pos = Pos + mirrorToFromWhomPara - mirrorToFromWhomPerp;
            
        }
    }
}
