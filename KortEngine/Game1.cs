using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace KortEngine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Map map = new Map();

        //old keyboard-last loop
        KeyboardState ksold;

        const int DiagMenu = 0;
        const int InGame = 1;
        int GameState = DiagMenu;

        //camera for drawing
        public Scythe.Camera2d cam = new Scythe.Camera2d();

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
            StartNewGame(1);

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

            Fonts.Load(Content);
            map.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        void StartNewGame(int maptype)
        {
            map.GenerateNewMap(maptype);
            map.Players.Add(new Player("Test", GetNewPlayerIndex(), new Vector2(0, 0)));
            cam.Pos = new Vector2(300, 600);
        }
        void LoadPlayers()
        {
            for (int i = 0; i < map.Players.Count; i++)
                map.Players[i].LoadContent(Content);
        }
        int GetNewPlayerIndex()
        {
            return map.Players.Count;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState ks = Keyboard.GetState();
            switch (GameState)
            {
                case DiagMenu:
                    if (ks.IsKeyDown(Keys.Q) & ksold.IsKeyUp(Keys.Q))
                    {
                        LoadPlayers();
                        GameState = InGame;
                    }
                    break;
                case InGame:
                    if (ks.IsKeyDown(Keys.O) & ksold.IsKeyUp(Keys.O))
                    {
                        if (Diagnostics.DiagActive == true)
                        {
                            Diagnostics.DiagActive = false;
                            //this.TargetElapsedTime = System.TimeSpan.FromSeconds(1f / 60f);
                        }
                        else
                        {
                            //this.TargetElapsedTime = System.TimeSpan.FromSeconds(1f / 30f);
                            Diagnostics.DiagActive = true;
                        }
                    }
                    if (ks.IsKeyDown(Keys.A))
                        cam.Move(new Vector2(-40, 0));
                    if (ks.IsKeyDown(Keys.D))
                        cam.Move(new Vector2(40, 0));
                    if (ks.IsKeyDown(Keys.W))
                        cam.Move(new Vector2(0, -40));
                    if (ks.IsKeyDown(Keys.S))
                        cam.Move(new Vector2(0, 40));

                    if (Diagnostics.DiagActive == true)
                    {
                        cam.Zoom = 0.4f;
                    }
                    else
                    {
                        cam.Zoom = 1f;
                    }
                    map.Update(ks,ksold, cam.Pos.X);
                    break;
            }

            ksold = ks;

            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            {

                switch (GameState)
                {
                    case DiagMenu:
                        break;
                    case InGame:
                        //Camera 2D begin
                        spriteBatch.Begin(SpriteSortMode.Deferred,
                            null, null, null, null, null,
                            cam.get_transformation(GraphicsDevice));
                        map.Draw(spriteBatch, GraphicsDevice);
                        spriteBatch.End();

                        spriteBatch.Begin();
                        //fps counter
                        FrameCounter.Draw(spriteBatch, gameTime);
                        spriteBatch.End();
                        break;
                }
            }

            base.Draw(gameTime);
        }
    }
}
