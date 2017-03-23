using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingSpree
{
    public class Camera
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
        /// The 3D position of the camera relative to the target
        /// </summary>
        Vector3 position;

        /// <summary>
        /// camera rotation around target
        /// </summary>
        float rotX, rotY;


        /// <summary>
        /// Target that camera is looking at
        /// </summary>
        GameObject target;

        public Camera()
        {
            position = new Vector3(0, 10, -10);
            fov = MathHelper.PiOver4;
            rotY = 0;
            rotX = 0;
        }

        public Vector3 getPos()
        {
            return (
                Vector3.Transform(
                    position,
                    Matrix.CreateRotationX(rotX) *
                    Matrix.CreateRotationY(rotY)
                ) + target.Pos
            );
        }

        public Matrix getViewMatrix()
        {
            Vector3 camPos = getPos();
            // cam pos, target pos, up direction
            return Matrix.CreateLookAt(
                camPos,
                target.Pos,
                Vector3.Up);
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

        public Vector3 getLookAt()
        {
            Vector3 lookAt = target.Pos - getPos();
            lookAt.Normalize();

            return lookAt;
        }

        public void MoveCamera(Vector2 byHowMuch)
        {
            rotY += byHowMuch.X;
            rotX += -1 * byHowMuch.Y;
            rotX = MathHelper.Clamp(
                rotX,
                -1.65f,
                .520f);
        }



        //getters and setters
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public GameObject Target
        {
            get { return target; }
            set { target = value; }
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
