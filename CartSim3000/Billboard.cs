﻿using Microsoft.Xna.Framework;
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

        Texture2D[] frames;
        int curFrame = 0;
        int count = 1;
        int framesX = 1;
        int framesY = 1;

        Texture2D texture;
        BasicEffect effect;

        GraphicsDevice graphics;

        public Vector3 Target = Vector3.Zero;

        //jumping parameters
        float floorHeight = 0;
        float timeBetweenJumpsMean = 1;
        float timeBetweenJumpsSD = 2f;
        float jumpInitVelMean = 1;
        float jumpInitVelSD = 2f;

        float initJumpOffset = 0f;
        float timeBetweenJumps = 1;
        float jumpInitVel = 1;
        double lastJumpTime = 0;
        bool jumping = false;

        public int CurFrame
        {
            get { return curFrame; }
            set {
                curFrame = value % count;
                generateData();
            }
        }

        public Billboard(Texture2D tex, GraphicsDevice graphics, Vector3 position, Quaternion quat, float scale)
            : base(position, quat, scale, null)
        {
            this.graphics = graphics;

            floorHeight = position.Y;

            texture = tex;
            effect = new BasicEffect(graphics);
            generateData();

            initJumpOffset = (float)r.NextDouble();
        }

        // animated billboard constructor
        public Billboard(Texture2D tex, GraphicsDevice graphics, int framesX, int framesY, int count, Vector3 position, Quaternion quat, float scale)
            : base(position, quat, scale, null)
        {
            this.graphics = graphics;

            floorHeight = position.Y;

            this.framesX = framesX;
            this.framesY = framesY;
            this.count = count;

            texture = tex;
            effect = new BasicEffect(graphics);
            generateData();

            initJumpOffset = (float)r.NextDouble();
        }

        public void Update(GameTime gameTime)
        {
            if (jumping)
            {
                //fall back down
                Vel += Vector3.Up * -9.8f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Pos += Vel * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(Pos.Y < floorHeight)
                {
                    jumping = false;
                    Pos = new Vector3(Pos.X, floorHeight, Pos.Z);

                    //generate params for next jump
                    lastJumpTime = gameTime.TotalGameTime.TotalSeconds;
                    timeBetweenJumps = genRandNum(timeBetweenJumpsMean, timeBetweenJumpsSD);
                    jumpInitVel = genRandNum(jumpInitVelMean, jumpInitVelSD);
                }

                CurFrame = (int)gameTime.TotalGameTime.TotalSeconds;
            }
            else
            {
                //if not jumping, then consider next jump
                if(gameTime.TotalGameTime.TotalSeconds - lastJumpTime > timeBetweenJumps && gameTime.TotalGameTime.TotalSeconds > initJumpOffset)
                {
                    //jump
                    jumping = true;
                    Vel = Vector3.Up * jumpInitVel;
                }
            }

            //turn to face target
            Vector3 myPos = new Vector3(Pos.X, 0, Pos.Z);
            Vector3 targetPos = new Vector3(Target.X, 0, Target.Z);
            Vector3 directionVector = Vector3.Normalize(targetPos - myPos);

            Vector3 axis = Vector3.Up;
            float angle = 0f;
            if (!directionVector.Equals(Vector3.Forward) && !directionVector.Equals(Vector3.Zero))
            {
                axis = Vector3.Normalize(Vector3.Cross(Vector3.Forward, directionVector));
                angle = (float)Math.Acos(Vector3.Dot(Vector3.Forward, directionVector));
            }

            Rotation = Quaternion.Slerp(Rotation, Quaternion.CreateFromAxisAngle(axis, angle), .01f);
        }

        static Random r = new Random();
        private float genRandNum(float mean, float sd)
        {
            //uniform dist
            //http://www.itl.nist.gov/div898/handbook/eda/section3/eda3662.htm
            double width = Math.Sqrt(12 * sd * sd);

            double x = r.NextDouble() - 0.5d;

            return (float)(x * width + mean);
        }

        public void generateData()
        {
            float aspectRatio = (texture.Width/2f)/ (float)texture.Height;

            float frameWidth = texture.Width / framesX;
            float frameHeight = texture.Height / framesY;
            int frameXind = curFrame % framesX;
            int frameYind = curFrame / framesX;
            if(curFrame > 0)
            {
                 Vector2 x = new Vector2(frameXind / framesX, (frameYind + 1) / framesY);
            }

            verts = new VertexPositionNormalTexture[]
            {
                //front
                new VertexPositionNormalTexture(new Vector3(-aspectRatio/2, 0, 0), new Vector3(1, 0, 0), new Vector2(frameXind/(float)framesX, (frameYind+1)/(float)framesY) ),
                new VertexPositionNormalTexture(new Vector3(aspectRatio/2, 0, 0), new Vector3(1, 0, 0), new Vector2((frameXind + .5f)/framesX, (frameYind+1)/(float)framesY) ),
                new VertexPositionNormalTexture(new Vector3(aspectRatio/2, 1, 0), new Vector3(1, 0, 0), new Vector2((frameXind + .5f)/framesX, frameYind/(float)framesY) ),
                new VertexPositionNormalTexture(new Vector3(-aspectRatio/2, 1, 0), new Vector3(1, 0, 0), new Vector2(frameXind/(float)framesX, frameYind/(float)framesY) ),

                //back 
                new VertexPositionNormalTexture(new Vector3(aspectRatio/2, 0, 0), new Vector3(1, 0, 0), new Vector2((frameXind + .5f)/framesX, (frameYind+1)/(float)framesY) ),
                new VertexPositionNormalTexture(new Vector3(-aspectRatio/2, 0, 0), new Vector3(1, 0, 0), new Vector2((frameXind + 1)/(float)framesX, (frameYind+1)/(float)framesY) ),
                new VertexPositionNormalTexture(new Vector3(-aspectRatio/2, 1, 0), new Vector3(1, 0, 0), new Vector2((frameXind + 1)/(float)framesX, frameYind/(float)framesY) ),
                new VertexPositionNormalTexture(new Vector3(aspectRatio/2, 1, 0), new Vector3(1, 0, 0), new Vector2((frameXind + .5f)/framesX,  frameYind/(float)framesY) )
            };

            idxs = new short[]
            {
                0,1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7
            };
        }

        public override void Draw(GameTime gameTime, Camera cam, Lamp lamp)
        {
            Lamp lamp2 = new Lamp(new Vector3(-30, 30, -30));

            effect.World = this.World;
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
