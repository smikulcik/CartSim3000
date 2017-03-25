using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ShoppingSpree
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        static Dictionary<string, Model> models = new Dictionary<string, Model>();

        public static Dictionary<string, Model> Models
        {
            get { return models; }
        }

        Camera cam;
        Lamp lamp;

        Point windowCenter;
        float sensitivity = 0.01f;

        List<GameObject> gameObjects;
        List<GameObject> shelves;

        // game specific vars
        GameObject floor, cart;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {


            gameObjects = new List<GameObject>();
            shelves = new List<GameObject>();
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            models["floor"] = Content.Load<Model>("floor");
            models["cerealBox"] = Content.Load<Model>("cerealBox");
            models["cart"] = Content.Load<Model>("cart");
            models["shelf"] = Content.Load<Model>("shelf");


            // Add game specific objectss
            floor = new GameObject(new Vector3(0, -1f, 0), Quaternion.Identity, 1f, models["floor"]);
            floor.Immovable = true;


            cart = new GameObject(new Vector3(0, .1f, 1), Quaternion.Identity, .01f, models["cart"]);

            //cart.Collider.EnableBounce = false;
            cart.Collider.EnableRotate = false;

            for (int i = 6; i < 100; i++)
            {
                gameObjects.Add(new GameObject(new Vector3(2, 2*i, 1), Quaternion.Identity, 0.01f, models["cerealBox"]));
            }

            for (int i = 0; i < 10; i++)
            {
                GameObject shelf = new GameObject(new Vector3(10, 2.5f, 10 * i), Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi), 0.01f, models["shelf"]);
                shelf.Immovable = true;

                shelves.Add(shelf);
            }
            for (int i = 0; i < 10; i++)
            {
                GameObject shelf = new GameObject(new Vector3(-10, 2.5f, 10 * i), Quaternion.Identity, 0.01f, models["shelf"]);
                shelf.Immovable = true;

                shelves.Add(shelf);
            }

            // generic games
            cam = new Camera(new Vector3(0, 0, 0), MathHelper.Pi);
            lamp = new Lamp(new Vector3(30, 30, 30));
            windowCenter = new Point(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        Boolean escapeDown = false;
        Boolean leftClickDown = false;
        Boolean rightClickDown = false;
        Boolean eKeyDown = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // don't update if screen not in focus
            if (!this.IsActive)
                return;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            //update world
            foreach (GameObject g in gameObjects)
            {
                g.Update(gameTime);
            }

            Point changeInMouse = windowCenter - Mouse.GetState().Position;

            cart.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, changeInMouse.X * sensitivity);
            //Quaternion r = cart.Rotation;
            cart.Update(gameTime);
            //cart.Rotation = r;
            //cart.Pos = new Vector3(cart.Pos.X, 5, cart.Pos.Z);

            #region Update clock
            /*
            secondsLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (secondsLeft <= 0 && !gameOver)
            {
                gameOver = true;
                if (score > 50)
                {
                    tc.AchieveTrophy(Trophies.win);
                }
                if (score == 314)
                {
                    tc.AchieveTrophy(Trophies.pi);
                }
                if (score >= 500)
                {
                    tc.AchieveTrophy(Trophies.fivehundredplus);
                }
            }
            if (gameOver)
            {
                secondsLeft = 0;
                return;
            }

            poisonedSecondsLeft = MathHelper.Max(poisonedSecondsLeft - (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
            if (poisonedSecondsLeft > 0)
                cam.Fov = MathHelper.PiOver2 * 4 / 3;
            else
                cam.Fov = MathHelper.PiOver4;


            slowedTimeLeft = MathHelper.Max(slowedTimeLeft - (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
            if (slowedTimeLeft > 0)
                playerSpeed = 0.5f;
            else
                playerSpeed = 1;

    
            */
            #endregion
            #region update camera parameters
            try
            {

                Mouse.SetPosition(windowCenter.X, windowCenter.Y);
            }
            catch (NullReferenceException)
            {
                //This gets thrown unexpectedly when you press escape
            }

            cam.MoveCamera(
                new Vector2(
                    changeInMouse.X * sensitivity,
                    changeInMouse.Y * sensitivity
                )
            );
            #endregion


            #region handle mouse clicks
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (leftClickDown == false)
                {
                    // On left mouse pressed
                    Console.WriteLine("L Mouse Pressed");

                }
                leftClickDown = true;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (leftClickDown == true)
                {
                    // On left mouse release
                    Console.WriteLine("L Mouse Released");

                }
                leftClickDown = false;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                rightClickDown = true;
            }
            if (Mouse.GetState().RightButton == ButtonState.Released)
            {
                if (rightClickDown == true)
                {
                    // On right mouse release
                    Console.WriteLine("R Mouse Released");

                }
                rightClickDown = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                if (eKeyDown == false)
                {
                    // On e key release
                    Console.WriteLine("E key pressed");
                }
                eKeyDown = true;
            }
            if (!Keyboard.GetState().IsKeyDown(Keys.E))
            {
                eKeyDown = false;
            }
            #endregion


            floor.Collider.checkCollision(cart);
            //collide with floor and cart
            foreach (GameObject box in gameObjects)
            {

                float angVelRadians = 2 * (float)Math.Atan2(box.AngVel.ToVector4().Length(), box.AngVel.W);
                //if (box.Vel.Length() > 0.001 || angVelRadians > 0.001)  // prune objects that arn't moving
                    floor.Collider.checkCollision(box);

                cart.Collider.checkCollision(box);
            }
            
            foreach (GameObject shelf in shelves)
            {
                shelf.Collider.checkCollision(cart);
            }
            
            //collide with boxes
           foreach (GameObject box1 in gameObjects)
            {
                foreach (GameObject box2 in gameObjects)
                {
                    if(box1 != box2)
                    {
                        box1.Collider.checkCollision(box2);
                    }
                }
            }

            #region move player

            Vector3 forward = Vector3.Transform(Vector3.UnitZ, Matrix.CreateFromQuaternion(cart.Rotation));
            Vector3 right = Vector3.Cross(forward, Vector3.Up);

            Vector3 movement = Vector3.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                movement -= right;
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
                movement += right;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                movement += forward;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
                movement -= forward;

            if (movement.Length() > 0)
            {
                movement.Normalize();
                movement *= .3f;
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    movement *= 2;//shift to run
                cart.Pos += movement;
            }
            cam.Pos = cart.Pos + forward * -2 + new Vector3(0, 5, 0);
            #endregion


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (GameObject g in gameObjects)
            {
                g.Draw(gameTime, cam, lamp);
            }
            foreach (GameObject s in shelves)
            {
                s.Draw(gameTime, cam, lamp);
            }
            floor.Draw(gameTime, cam, lamp);
            cart.Draw(gameTime, cam, lamp);

            //draw bounding sphere
            //Vector3 cartCenter = (cart.Collider.bb.Max + cart.Collider.bb.Min) / 2;
            //float cartRad = (cart.Collider.bb.Max - cart.Collider.bb.Min).Length() / 2;
            //BoundingSphereOverlay.Draw(cartCenter, cartRad, GraphicsDevice, cam);
            

            base.Draw(gameTime);
        }
    }
}
