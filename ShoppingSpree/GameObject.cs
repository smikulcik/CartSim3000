using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingSpree
{
    /// <summary>
    /// Generic objects in game, can be a character, an empty, 
    /// a lamp, an item, etc.
    /// </summary>
    public class GameObject
    {
        protected Vector3 position;
        protected float scale;
        protected float rotation;
        protected string modelName;
        protected float radius;

        protected Vector3 velocity;


        //constructors
        public GameObject()
        {
            position = new Vector3(0, 0, 0);
            scale = 1f;
            rotation = 0f;
            modelName = "";
            radius = 5;
        }
        public GameObject(Vector3 position, float rotation, float scale, string model)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.modelName = model;
            radius = 5;
        }

        /// <summary>
        /// Draw game object
        /// </summary>
        public virtual void Draw(GameTime gameTime, Camera cam, Lamp lamp)
        {
            // null for empty game object
            if (modelName != null)
            {
                this.Model.Draw(
                    this.World,
                    cam.getViewMatrix(),
                    cam.getProjMatrix()
                );
            }

        }
        
        public virtual void Draw(GameTime gameTime, Camera cam)
        {
            Draw(gameTime, cam, null);
        }

        public virtual void Update(GameTime gameTime)
        {

        }
        public virtual void onCollide() { }


        // getters and setters
        public Vector3 Pos
        {
            get { return position; }
            set { position = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public string ModelName
        {
            get { return modelName; }
            set { modelName = value; }
        }
        public Model Model
        {
            get
            {
                if (modelName != null)
                {
                    return Game1.Models[modelName];
                }
                return null;
            }
        }
        public float Radius
        {
            get { return radius; }
        }
        public Vector3 Vel
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public Matrix World
        {
            get
            {
                return (
                    Matrix.CreateScale(scale) *
                    Matrix.CreateRotationY(rotation) *
                    Matrix.CreateTranslation(position)
                );
            }
        }


    }
}
