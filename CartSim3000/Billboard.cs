using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartSim3000
{
    class Billboard : GameObject
    {

        VertexPositionNormalTexture[] verts;
        short[] idxs;

        Texture2D texture;
        BasicEffect effect;

        GraphicsDevice graphics;

        public Billboard(Texture2D tex, GraphicsDevice graphics, Vector3 position, Quaternion quat, float scale)
            : base(position, quat, scale, null)
        {
            this.graphics = graphics;

            texture = tex;
            effect = new BasicEffect(graphics);
            generateData();
        }

        public void generateData()
        {
            float aspectRatio = (texture.Width/2f)/ (float)texture.Height;

            verts = new VertexPositionNormalTexture[]
            {
                //front
                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector2(0,1) ),
                new VertexPositionNormalTexture(new Vector3(aspectRatio, 0, 0), new Vector3(1, 0, 0), new Vector2(.5f,1) ),
                new VertexPositionNormalTexture(new Vector3(aspectRatio, 1, 0), new Vector3(1, 0, 0), new Vector2(.5f,0) ),
                new VertexPositionNormalTexture(new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector2(0,0) ),

                //back
                new VertexPositionNormalTexture(new Vector3(aspectRatio, 0, 0), new Vector3(1, 0, 0), new Vector2(.5f,1) ),
                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector2(1,1) ),
                new VertexPositionNormalTexture(new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector2(1,0) ),
                new VertexPositionNormalTexture(new Vector3(aspectRatio, 1, 0), new Vector3(1, 0, 0), new Vector2(.5f,0) )
            };

            idxs = new short[]
            {
                0,1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7
            };
        }

        public override void Draw(GameTime gameTime, Camera cam, Lamp lamp)
        {
            Lamp lamp2 = new Lamp(new Vector3(-30, 30, -30));

            effect.World = this.World*Matrix.CreateScale(5);
            effect.Projection = cam.getProjMatrix();
            effect.View = cam.getViewMatrix();
            /*effect.EnableDefaultLighting();

            effect.DirectionalLight0.Direction = Vector3.Normalize(Pos - lamp.Pos);
            effect.DirectionalLight1.Direction = Vector3.Normalize(Pos - lamp2.Pos);
            effect.DirectionalLight1.DiffuseColor = Color.White.ToVector3();
            effect.DirectionalLight2.Enabled = false;*/
            effect.TextureEnabled = true;
            effect.Texture = texture;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    verts, 0, 8,
                    idxs, 0, 4
                );
            }
        }
    }
}
