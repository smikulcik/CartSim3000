using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingSpree
{
    public class GameObject
    {
        private Vector3 position;
        private Vector3 velocity;
        private Quaternion rotation;
        private Quaternion angVel;
        private float scale = 1;
        private Model model;
        private BoundingBoxCollider collider;
        private bool immovable = false;

        public GameObject()
        {
            Pos = new Vector3(0, 0, 0);
            Vel = new Vector3(0, 0, 0);
            rotation = Quaternion.Identity;
            angVel = Quaternion.Identity;
            scale = 1f;
            model = null;
        }
        public GameObject(Vector3 position, Quaternion quat, float scale, Model model)
        {
            this.Pos = position;
            Vel = new Vector3(0, 0, 0);
            angVel = Quaternion.CreateFromYawPitchRoll(0, 0, 0);
            this.rotation = quat;
            this.scale = scale;
            this.model = model;

            if(model != null)
                collider = new BoundingBoxCollider(this);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!immovable)
            {
                Vel += Vector3.Up * -9.8f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Pos += Vel * (float)gameTime.ElapsedGameTime.TotalSeconds;

                rotation = Quaternion.Lerp(rotation, rotation * angVel, (float)gameTime.ElapsedGameTime.TotalSeconds);

                //apply drag
                angVel = Quaternion.Lerp(angVel, Quaternion.Identity, .0001f);
                
            }
            collider.Update();
        }

        public virtual void Draw(GameTime gameTime, Camera cam, Lamp lamp)
        {
            model.Draw(World, cam.getViewMatrix(), cam.getProjMatrix());
            //Matrix.CreateScale(scale)*Matrix.CreateFromQuaternion(quaternion)*Matrix.CreateTranslation(position)
            // null for empty game object
            if (model != null)
            {
                foreach(ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect e in mesh.Effects)
                    {
                        e.World = this.World;
                        e.Projection = cam.getProjMatrix();
                        e.View = cam.getViewMatrix();
                        e.EnableDefaultLighting();

                        e.DirectionalLight0.Direction = Vector3.Normalize(Pos - lamp.Pos);
                        e.DirectionalLight1.Enabled = false;
                        e.DirectionalLight2.Enabled = false;
                    }
                    //mesh.Draw();
                }
            }else
            {
                Debug.WriteLine(model);
            }

            //draw bounding box

            //BoundingBoxOverlay.Draw(Collider.bb, GraphicsDevice, cam);
        }

        public Vector3 Pos
        {
            get { return position; }
            set {
                if (float.IsNaN(value.X) || float.IsNaN(value.Y) || float.IsNaN(value.Z))
                    throw new ArgumentOutOfRangeException("must not be NaN");
                position = value; }
        }
        public Vector3 Vel
        {
            get { return velocity; }
            set {
                if (float.IsNaN(value.X) || float.IsNaN(value.Y) || float.IsNaN(value.Z))
                    throw new ArgumentOutOfRangeException("must not be NaN");
                velocity = value; }
        }
        public Quaternion Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Quaternion AngVel
        {
            get { return angVel; }
            set { angVel = value; }
        }
        public Model Model
        {
            get { return model; }
            set { model = value; }
        }
        public BoundingBoxCollider Collider
        {
            get { return collider; }
            set { collider = value; }
        }

        public float Radius
        {
            get {
                if (model != null) {
                    return model.Meshes[0].BoundingSphere.Radius;
                }else
                {
                    return 0f;
                }
            }
        }
        public Matrix World
        {
            get
            {
                return (
                    Matrix.CreateScale(scale) *
                    Matrix.CreateFromQuaternion(rotation) *
                    Matrix.CreateTranslation(position)
                );
            }
        }

        public bool Immovable
        {
            get
            {
                return immovable;
            }
            set
            {
                immovable = value;
            }
        }

    }
}
