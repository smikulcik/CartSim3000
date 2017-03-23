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

        Point windowCenter;
        float sensitivity = 0.01f;

        List<GameObject> gameObjects;

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
            
            // Add game specific objectss
            floor = new GameObject(new Vector3(0, 0, 0), 0, 1f, "floor");
            gameObjects.Add(floor);

            cart = new GameObject(new Vector3(0, 0, 0), 0, .01f, "cart");
            gameObjects.Add(cart);

            // generic games
            cam = new Camera();
            cam.Target = cart;
            windowCenter = new Point(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

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
            /*
            #region Update clock
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


            #endregion
            */
            #region update camera parameters
            Point changeInMouse = windowCenter - Mouse.GetState().Position;
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

            cart.Rotation += changeInMouse.X * sensitivity;
            #endregion

            #region move player

            Vector3 forward = Vector3.Transform(Vector3.UnitZ, Matrix.CreateRotationY(cart.Rotation));
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
                //movement *= playerSpeed;
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    movement *= 2;//shift to run
                cart.Pos += movement;
            }
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


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            foreach(GameObject g in gameObjects)
            {
                g.Draw(gameTime, cam);
            }

            base.Draw(gameTime);
        }
    }
}
