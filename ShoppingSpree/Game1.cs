using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

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

        float jumpOnCartTransistion = 0;

        Point windowCenter;
        float sensitivity = 0.01f;

        List<GameObject> gameObjects;
        List<GameObject> shelves;

        SpriteFont letterFont;

        // game specific vars
        GameObject floor, cart;
        GameObject[] ceiling;
        GameObject walls;
        GameObject larm, rarm;

        //animators
        LArmAnimations larmAnimator;
        RArmAnimations rarmAnimator;

        // state vars
        float timeLeft = 30f;
        int score = 0; //score in number of boxes in cart
        enum GameState
        {
            TITLECREDITS,
            HOWTOPLAY,
            GAME,
            GAMEOVER
        };
        GameState gameState = GameState.TITLECREDITS;

        //Sound effects
        SoundEffect cartSound;
        SoundEffectInstance cartSoundInstance;

        SoundEffect cartSqueak;
        SoundEffectInstance cartSqueakInstance;

        SoundEffect gruntSound;

        SoundEffect cardboardBox;

        Song jazzSong;

        Scoreboard scoreboard;
        string scoreboardFilename = "scoreboard.xml";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            scoreboard = new Scoreboard();
            try
            {
                scoreboard.LoadFromXML(scoreboardFilename);
            }
            catch(FileNotFoundException e)
            {
                // no save file was found, a new one will be generated
                //pass
            }
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

            letterFont = Content.Load<SpriteFont>("LetterFont");

            cartSound = Content.Load<SoundEffect>("WheelingACart");
            cartSoundInstance = cartSound.CreateInstance();

            cartSqueak = Content.Load<SoundEffect>("Squeak");
            cartSqueakInstance = cartSqueak.CreateInstance();

            gruntSound = Content.Load<SoundEffect>("grunt");

            jazzSong = Content.Load<Song>("Jazz club1 130");
            MediaPlayer.IsRepeating = true;
            
            cardboardBox = Content.Load<SoundEffect>("cardboardBox");

            models["floor"] = Content.Load<Model>("floor");
            models["walls"] = Content.Load<Model>("walls");
            models["ceiling"] = Content.Load<Model>("ceiling");
            models["cerealBox"] = Content.Load<Model>("cerealBox");
            models["cerealBoxBlue"] = Content.Load<Model>("cerealBoxBlue");
            models["cart"] = Content.Load<Model>("cart");
            models["shelf"] = Content.Load<Model>("shelf");
            models["Larm"] = Content.Load<Model>("Larm");
            models["Rarm"] = Content.Load<Model>("Rarm");


            // Add game specific objectss
            floor = new GameObject(new Vector3(0, -1f, 0), Quaternion.Identity, 1f, models["floor"]);
            floor.Immovable = true;

            walls = new GameObject(new Vector3(0, 0, 0), Quaternion.Identity, .5f, models["walls"]);

            ceiling = new GameObject[6 * 6];
            for(int i = 0; i < 6; i++)
            {
                for(int j = 0; j < 6; j++)
                {
                    ceiling[6 * i + j] = new GameObject(new Vector3(-39+i*14, 10, -28+j*15), Quaternion.Identity, .01f, models["ceiling"]);
                }
            }


            cart = new GameObject(new Vector3(0, .1f, 1), Quaternion.Identity, .01f, models["cart"]);

            //cart.Collider.EnableBounce = false;
            cart.Collider.EnableRotate = false;
            cart.Collider.UseMeshBoundingBox = false;
            cart.Collider.BBGroup = new BoundingBox[]{ // construct collision boxes for cart
                new BoundingBox(new Vector3(-2f, 0, -1), new Vector3(2, .8f, 6.5f)),
                new BoundingBox(new Vector3(-2f, .8f, -1), new Vector3(-1.5f, 3f, 6.5f)),
                new BoundingBox(new Vector3(1.5f, .8f, -1), new Vector3(2f, 3f, 6.5f)),
                new BoundingBox(new Vector3(-1.5f, .8f, -1), new Vector3(1.5f, 3f, -.5f)),
                new BoundingBox(new Vector3(-1.5f, .8f, 5.5f), new Vector3(1.5f, 3f, 6.5f))
            };

            //arms
            larm = new GameObject(
                new Vector3(0, 0, 0), LArmAnimations.slerp(0),
                .01f,
                models["Larm"]);
            rarm = new GameObject(
                new Vector3(0, 0, 0), RArmAnimations.slerp(0),
                .01f,
                models["Rarm"]);

            larmAnimator = new LArmAnimations(larm);
            rarmAnimator = new RArmAnimations(rarm);

            Random r = new Random();

            // build shelves
            for (int shelfno = 0; shelfno < 4; shelfno++)
            {
                //add shelf
                GameObject shelf = new GameObject(new Vector3(5, 2.5f, 10 * shelfno), Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi), 0.01f, models["shelf"]);
                shelf.Collider.UseMeshBoundingBox = false;
                shelf.Collider.EnableRotate = false;
                BoundingBox[] shelfBBs = new BoundingBox[]{
                    new BoundingBox(new Vector3(-1, -2.55f, -2.7f), new Vector3(1, -2.35f, 2.8f)),
                    new BoundingBox(new Vector3(-1, -1.2f, -2.7f), new Vector3(1, -.95f, 2.8f)),
                    new BoundingBox(new Vector3(-1, 0f, -2.7f), new Vector3(1, .3f, 2.8f)),
                    new BoundingBox(new Vector3(-1, 1.3f, -2.7f), new Vector3(1, 1.5f, 2.8f)),
                    new BoundingBox(new Vector3(.2f, -3f, -2.7f), new Vector3(.9f, 2.2f, 2.8f)),
                };
                shelf.Collider.BBGroup = shelfBBs;
                shelves.Add(shelf);

                GameObject shelf2 = new GameObject(new Vector3(-5, 2.5f, 10 * shelfno), Quaternion.Identity, 0.01f, models["shelf"]);
                shelf2.Collider.UseMeshBoundingBox = false;
                shelf2.Collider.EnableRotate = false;
                BoundingBox[] shelf2BBs = new BoundingBox[]{
                    new BoundingBox(new Vector3(-1, -2.55f, -2.7f), new Vector3(1, -2.35f, 2.8f)),
                    new BoundingBox(new Vector3(-1, -1.2f, -2.7f), new Vector3(1, -.95f, 2.8f)),
                    new BoundingBox(new Vector3(-1, 0f, -2.7f), new Vector3(1, .3f, 2.8f)),
                    new BoundingBox(new Vector3(-1, 1.3f, -2.7f), new Vector3(1, 1.5f, 2.8f)),
                    new BoundingBox(new Vector3(-.2f, -3f, -2.7f), new Vector3(-.9f, 2.2f, 2.8f)),
                };
                shelf2.Collider.BBGroup = shelf2BBs;
                shelves.Add(shelf2);

                for (int shelfLevels = 0; shelfLevels < 4; shelfLevels++)
                {
                    BoundingBox s = shelfBBs[shelfLevels];
                    //add boxes on shelf
                    for (int col = 0; col < 4; col++)
                    {
                        Model boxModel;
                        if(r.Next() % 10 == 0)
                        {
                            boxModel = models["cerealBoxBlue"];
                        }
                        else
                        {
                            boxModel = models["cerealBox"];
                        }

                        GameObject box = new GameObject(
                            shelf.Pos +
                            new Vector3(s.Min.X, s.Max.Y, s.Min.Z) +
                            new Vector3(.1f, 0, col * 1.5f + .5f),
                            Quaternion.Identity, 0.01f, boxModel
                        );
                        box.Collider.EnableRotate = false;
                        gameObjects.Add(box);


                        GameObject box2 = new GameObject(
                            shelf2.Pos +
                            new Vector3(s.Max.X, s.Max.Y, s.Min.Z) +
                            new Vector3(-.1f, .1f, col * 1.5f + .5f),
                            Quaternion.Identity, 0.01f, boxModel
                        );
                        box2.Collider.EnableRotate = false;
                        gameObjects.Add(box2);
                    }
                }
            }
            /*
            for (int i = 0; i < 10; i++)
            {
                GameObject shelf = new GameObject(new Vector3(-10, 2.5f, 10 * i), Quaternion.Identity, 0.01f, models["shelf"]);
                shelf.Immovable = true;

                shelves.Add(shelf);
            }*/

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

        Boolean wasActiveLastUpdate = false;
        Boolean spaceDown = false;
        Boolean leftClickDown = false;
        Boolean rightClickDown = false;
        Boolean enterDown = false;
        Boolean eKeyDown = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // don't update if screen not in focus
            if (!this.IsActive) {
                wasActiveLastUpdate = false;
                return;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (gameState == GameState.TITLECREDITS)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    enterDown = true;
                }
                else
                {
                    if (enterDown)
                    {
                        gameState = GameState.HOWTOPLAY;
                    }
                    enterDown = false;
                }
                return;
            }

            if (gameState == GameState.HOWTOPLAY)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    enterDown = true;
                }
                else
                {
                    if (enterDown)
                    {
                        gameState = GameState.GAME;
                        MediaPlayer.Play(jazzSong);
                    }
                    enterDown = false;
                }
                return;
            }
            if (gameState == GameState.GAMEOVER)
            {
                MediaPlayer.Stop();
                return;
            }

            timeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeLeft <= 0)
            {
                timeLeft = 0f;
                endGame();
                return;
            }

            Point changeInMouse = windowCenter - Mouse.GetState().Position;

            // Reset mouse if we just activated the game window.
            // Otherwise mouse jumps to center of screen and moves camera.
            if (!wasActiveLastUpdate)
            {
                Console.WriteLine("reset mouse");
                changeInMouse = new Point(0, 0);
            }
            #region  update world
            foreach (GameObject g in gameObjects)
            {
                g.Update(gameTime);
            }
            foreach (GameObject s in shelves)
            {
                s.Update(gameTime);
            }
            #endregion

            //cart.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, changeInMouse.X * sensitivity);
            cart.Update(gameTime);

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
                    larmAnimator.Play();
                    gruntSound.Play();

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
                    rarmAnimator.Play();
                    gruntSound.Play();

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

            #region Handle Collisions
            floor.Collider.checkCollision(cart);

            //handle collisions with the walls
            if (cart.Pos.X < -38)
                cart.Pos = new Vector3(-38, cart.Pos.Y, cart.Pos.Z);
            if (cart.Pos.X > 38)
                cart.Pos = new Vector3(38, cart.Pos.Y, cart.Pos.Z);
            if (cart.Pos.Z < -35)
                cart.Pos = new Vector3(cart.Pos.X, cart.Pos.Y, -35);
            if (cart.Pos.Z > 34)
                cart.Pos = new Vector3(cart.Pos.X, cart.Pos.Y, 34);

            // collide shelf with floor and shelf
            foreach (GameObject shelf in shelves)
            {

                floor.Collider.checkCollision(shelf);
                if (cart.Collider.checkCollision(shelf))
                {
                    
                }
            }
            bool hasPlayedBoxNoise = false;
            //collide boxes with boxes
           foreach (GameObject box1 in gameObjects)
            {
                foreach (GameObject box2 in gameObjects)
                {
                    if(box1 != box2)
                    {
                        if (box1.Collider.checkCollision(box2) && (box1.Vel - box2.Vel).Length() > 2 ) {
                            //if collides with enough speed
                            if (!hasPlayedBoxNoise)  // only one per frame
                            {
                                hasPlayedBoxNoise = true;
                                cardboardBox.Play();
                            }
                        }
                    }
                }
            }
            //collide boxes with floor, shelf and cart, and arms
            foreach (GameObject box in gameObjects)
            {
                floor.Collider.checkCollision(box);
                foreach (GameObject shelf in shelves)
                {
                    shelf.Collider.checkCollision(box);
                }
                //enable rotation when cart collides with box
                bool boxRotateState = box.Collider.EnableRotate;
                box.Collider.EnableRotate = true;
                cart.Collider.checkCollision(box);
                box.Collider.EnableRotate = boxRotateState;

                larm.Collider.checkCollision(box);
                rarm.Collider.checkCollision(box);
            }
            #endregion

            #region move player


            Vector3 forward = Vector3.Transform(Vector3.UnitZ, Matrix.CreateFromQuaternion(cart.Rotation));
            Vector3 right = Vector3.Cross(forward, Vector3.Up);

            float jumpDuration = .1f;//seconds
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (!spaceDown)
                {
                    cart.Vel = forward * 35;
                    cart.Pos += Vector3.Up*.2f;
                    spaceDown = true;
                 }
                jumpOnCartTransistion += (float)gameTime.ElapsedGameTime.TotalSeconds / jumpDuration;
            }
            else
            {
                jumpOnCartTransistion -= (float)gameTime.ElapsedGameTime.TotalSeconds / jumpDuration;
                spaceDown = false;
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
                    movement *= 3f;
                    cart.Vel += movement;
                    cart.Vel = Vector3.Normalize(cart.Vel) * MathHelper.Min(cart.Vel.Length(), 10);
                }
            }


            jumpOnCartTransistion = MathHelper.Clamp(jumpOnCartTransistion, 0, 1f);
            cam.Pos = Vector3.Lerp(
                cart.Pos + forward * -2 + new Vector3(0, 5, 0), //normal
                cart.Pos + forward * -1 + new Vector3(0, 6, 0), //jumpedOnCart
                jumpOnCartTransistion
            );

            larm.Pos = cart.Pos + new Vector3(2, 4, -2);
            rarm.Pos = cart.Pos + new Vector3(-2, 4, -2);


            larmAnimator.Update(gameTime);
            rarmAnimator.Update(gameTime);

            larm.Collider.BBGroup = new BoundingBox[] { BoundingBox.CreateFromPoints(larm.Collider.Verts) };
            rarm.Collider.BBGroup = new BoundingBox[] { BoundingBox.CreateFromPoints(rarm.Collider.Verts) };

            #endregion


            #region play sound effects
            float forwardVel = Vector3.Dot(forward, cart.Vel);
            if (Math.Abs(forwardVel) > .1)
            {
                if (cartSoundInstance.State != SoundState.Playing)
                    cartSoundInstance.Play();
            }
            else
                cartSoundInstance.Stop();


            float sideVel = Vector3.Dot(right, cart.Vel);
            if (Math.Abs(sideVel) > .1)
            {
                if (cartSqueakInstance.State != SoundState.Playing)
                    cartSqueakInstance.Play();
            }
            else
                cartSqueakInstance.Stop();
            #endregion

            UpdateScore();

            wasActiveLastUpdate = true;
            base.Update(gameTime);
        }

        private void UpdateScore()
        {
            int count = 0;
            // check each box to see if it is in the cart
            foreach(GameObject box in gameObjects)
            {
                //use cart bounding box for score calculation
                if (box.Pos.X > cart.Pos.X - 1.5 &&
                    box.Pos.X < cart.Pos.X + 1.5 &&
                    box.Pos.Y > cart.Pos.Y + 0.8 &&
                    box.Pos.Z > cart.Pos.Z - 0.5 &&
                    box.Pos.Z < cart.Pos.Z + 5.5)
                {
                    // 1 pt for yellow box, 5 pts for blue box
                    if (box.Model == models["cerealBox"])
                        count++;
                    else if (box.Model == models["cerealBoxBlue"])
                        count += 5;
                }
            }
            score = count;
        }

        private void endGame()
        {
            if(scoreboard.Scores.Count < scoreboard.Size || score > scoreboard.LowestScore)
            {
                NamePrompt p = new NamePrompt();
                p.ShowDialog();
                scoreboard.addScore(new ScoreboardEntry(p.name, score));
                scoreboard.SaveToXML(scoreboardFilename);
            }
            gameState = GameState.GAMEOVER;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.TITLECREDITS:
                    DrawTitleCredits(gameTime);
                    break;
                case GameState.HOWTOPLAY:
                    DrawHowToPlay(gameTime);
                    break;
                case GameState.GAME:
                    DrawGame(gameTime);
                    break;
                case GameState.GAMEOVER:
                    DrawGameOver(gameTime);
                    break;
            }
            base.Draw(gameTime);
        }

        private void DrawTitleCredits(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.DarkBlue);
            spriteBatch.Begin();

            spriteBatch.DrawString(
                letterFont,
                "Shopping Cart Sim",
                new Vector2(300, 20),
                Color.White
            );
            spriteBatch.DrawString(
                letterFont,
                "Hit Enter...",
                new Vector2(400, 400),
                Color.White
            );

            spriteBatch.DrawString(
                letterFont,
                "Credits:\n\n" +
                "Concept, Game Engine, and\n" +
                "   Programming: Simon Mikulcik\n" +
                "In partial fulfillment of the requirements\n" +
                "   for CSC 316 at Eastern Kentucky University\n\n" +
                "Made with Monogame",
                new Vector2(20, 100),
                Color.White);

            spriteBatch.End();
            //reset graphics device
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

        }

        private void DrawHowToPlay(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);
            spriteBatch.Begin();

            spriteBatch.DrawString(
                letterFont,
                "How To Play",
                new Vector2(300, 20),
                Color.White
            );
            spriteBatch.DrawString(
                letterFont,
                "Hit Enter to begin...",
                new Vector2(400, 400),
                Color.White
            );

            spriteBatch.DrawString(
                letterFont,
                "Objective: Collect boxes in cart to earn a high score\n\n" +
                "WASD: Movements\n" +
                "Mouse: Look Around\n" +
                "L/R Mouse Clicks: Use arms\n" +
                "Space: Jump on cart\n" +
                "Esc: Quit",
                new Vector2(20, 100),
                Color.White);

            spriteBatch.End();
            //reset graphics device
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

        }

        private void DrawGameOver(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);
            spriteBatch.Begin();

            spriteBatch.DrawString(
                letterFont,
                "Game Over!",
                new Vector2(300, 20),
                Color.White
            );
            spriteBatch.DrawString(
                letterFont,
                "Score: " + score,
                new Vector2(500, 20),
                Color.White
            );
            //draw scoreboard
            for(int i = 0; i < scoreboard.Size; i++)
            {
                spriteBatch.DrawString(
                    letterFont,
                    (i + 1) + ".",
                    new Vector2(100, 80 + i * 35),
                    Color.White
                );

                if (i < scoreboard.Scores.Count)
                {
                    spriteBatch.DrawString(
                        letterFont,
                        scoreboard.Scores[i].Name + " " + scoreboard.Scores[i].Value,
                        new Vector2(145, 80 + i*35),
                        Color.White
                    );
                }
            }

            spriteBatch.End();
            //reset graphics device
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

        }

        private void DrawGame(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (GameObject g in gameObjects)
            {
                g.Draw(gameTime, cam, lamp);
            }
            foreach (GameObject s in shelves)
            {
                s.Draw(gameTime, cam, lamp);

                //draw bounding box
                /*foreach (BoundingBox bb in s.Collider.BBGroup)
                {
                    BoundingBoxOverlay.Draw(bb, GraphicsDevice, cam);
                }*/
            }
            floor.Draw(gameTime, cam, lamp);

            walls.Draw(gameTime, cam, lamp);
            foreach(GameObject ceilSection in ceiling)
            {
                ceilSection.Draw(gameTime, cam, null);
            }
            cart.Draw(gameTime, cam, lamp);
            larm.Draw(gameTime, cam, lamp);
            rarm.Draw(gameTime, cam, lamp);

            /*foreach (BoundingBox bb in cart.Collider.BBGroup)
            {
                BoundingBoxOverlay.Draw(bb, GraphicsDevice, cam);
            }*/
            //draw bounding sphere
            //Vector3 cartCenter = (cart.Collider.bb.Max + cart.Collider.bb.Min) / 2;
            //float cartRad = (cart.Collider.bb.Max - cart.Collider.bb.Min).Length() / 2;
            //BoundingSphereOverlay.Draw(cartCenter, cartRad, GraphicsDevice, cam);

            // HUD
            spriteBatch.Begin();

            spriteBatch.DrawString(
                letterFont,
                "Time: " + Math.Floor(timeLeft),
                new Vector2(20, 20),
                Color.White
            );
            Vector3 horizontalVel = cart.Vel - (Vector3.Dot(Vector3.Up, cart.Vel)) * Vector3.Up;
            spriteBatch.DrawString(
                letterFont,
                "Speed: " + Math.Floor(horizontalVel.Length()) + " ft/s",
                new Vector2(20, 50),
                Color.White
            );

            spriteBatch.DrawString(
                letterFont,
                "Score: " + score,
                new Vector2(20, 80),
                Color.White
            );
            spriteBatch.End();
            //reset graphics device
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

        }
    }
}
