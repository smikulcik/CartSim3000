using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingSpree
{
    public class Camera: GameObject
    {
        /// <summary>
        /// field of view (radians)
        /// </summary>
        float fov;

        public float Fov
        {
            get { return fov; }
            set { fov = value; }
        }

        /// <summary>
        /// Clipping bounds
        /// </summary>
        float nearClip = 0.001f;
        float farClip = 3000.0f;

        /// <summary>
        /// Target that camera is looking at
        /// </summary>
        float rotX = 0;
        float rotY = 0;

        /// <summary>
        /// Rendering vars
        /// </summary>
        RenderTarget2D renderTarget;

        public Camera() 
        {
            fov = MathHelper.PiOver2;
        }
        public Camera(Vector3 position, float rotX, GraphicsDevice device) : base(position, Quaternion.CreateFromYawPitchRoll(rotX, 0, 0), 1f, null)
        {
            this.rotX = rotX;
            fov = MathHelper.PiOver2;
        }

        private void init(GraphicsDevice device)
        {
            PresentationParameters pp = device.PresentationParameters;
            renderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, true, device.DisplayMode.Format, DepthFormat.Depth24);
        }

        public Matrix getViewMatrix()
        {
            // cam pos, target pos, up direction
            return Matrix.CreateLookAt(Pos, Pos + Vector3.Transform(Vector3.Forward, Rotation), Vector3.Up);
        }
        public Matrix getProjMatrix()
        {
            return Matrix.CreatePerspectiveFieldOfView(
                fov,
                1,  // aspect ratio
                nearClip,
                farClip
            );
        }

        public void MoveCamera(Vector2 byHowMuch)
        {
            rotX += byHowMuch.X;
            rotY += byHowMuch.Y;
            rotY = MathHelper.Clamp(
                rotY,
                -1.25f,
                .520f);
            Rotation = Quaternion.CreateFromYawPitchRoll(rotX, rotY, 0);
        }

        /// <summary>
        /// Activate this camera as what we are drawing from
        /// </summary>
        /// <param name="device"></param>
        public void Activate(GraphicsDevice device)
        {
            init(device);
            device.SetRenderTarget(renderTarget);
        }

        /// <summary>
        /// Deactivate current camera and get the rendered texture
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public Texture2D Deactivate(GraphicsDevice device)
        {
            device.SetRenderTarget(null);
            return (Texture2D)renderTarget;
        }
        
        public float RotX
        {
            get { return rotX; }
            set { rotX = value; }
        }

        public float RotY
        {
            get { return rotX; }
            set { rotX = value; }
        }

    }
}
