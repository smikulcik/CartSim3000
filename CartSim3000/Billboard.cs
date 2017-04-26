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

        public Billboard(Texture2D tex, GraphicsDevice graphics)
        {
            texture = tex;
            effect = new BasicEffect(graphics);
            generateData();
        }

        public void generateData()
        {
            verts = new VertexPositionNormalTexture[]
            {
                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector2(0,1) ),
                new VertexPositionNormalTexture(new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector2(1,1) ),
                new VertexPositionNormalTexture(new Vector3(1, 1, 0), new Vector3(1, 0, 0), new Vector2(1,0) ),
                new VertexPositionNormalTexture(new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector2(0,0) )
            };

            idxs = new short[]
            {
                0,1, 2, 0, 2, 3
            };
        }

        public void Draw(GameTime gameTime, GraphicsDevice graphics, Camera cam, Lamp lamp)
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
                    verts, 0, 4,
                    idxs, 0, 2
                );
            }
        }
    }
}
