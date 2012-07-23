using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace Foldit3D
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameManager ourGame;
        public static float closeRate = 0.07f;
        public static float openRate = 0.04f;
        public static GraphicsDevice device;

        public static Camera camera;
        public static InputHandler input;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            XMLReader.Load("data.xml");
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            #region screenInit
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 680;
            //graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            // Window.AllowUserResizing = true; 
            Window.Title = "Let's Fold It!!!";
            this.IsMouseVisible = true;
            #endregion

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteFont font = Content.Load<SpriteFont>("font");
            SpriteFont scoreFont = Content.Load<SpriteFont>("scoreFont");
            HoleManager holeManager = new HoleManager(Content.Load<Texture2D>("hole2"), Content.Load<Effect>("effects"));
            PlayerManager playerManager = new PlayerManager(Content.Load<Texture2D>("gummy2"), Content.Load<Effect>("effects"));
            PowerUpManager powerupManager = new PowerUpManager(Content.Load<Texture2D>("inkspot"), Content.Load<Effect>("effects"));
            Board board = new Board(Content.Load<Texture2D>("paper"), Content.Load<Effect>("effects"));
            camera = new Camera(this);
            input = new InputHandler(this);
            ourGame = new GameManager(font, scoreFont, holeManager, playerManager, powerupManager,board);
            ourGame.loadCurrLevel();
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            ourGame.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            ourGame.Draw(gameTime, spriteBatch, graphics);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
