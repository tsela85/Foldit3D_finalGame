using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Foldit3D
{
    enum GameState { normal, folding, scored };

    class GameManager
    {
        SpriteFont font, scoreFont;
        static GameState gamestate;
        Board.BoardState boardstate;
        HoleManager holeManager;
        PlayerManager playerManager;
        PowerUpManager powerupManager;
        Board board;

        string win = "      EXCELLENT!!!\n you did it with: ";
        int level;
        int endLevel;
        int folds;
        int first = 1;

        ///////////////////////////
        List<IDictionary<string, string>> levels = new List<IDictionary<string, string>>();

        public GameManager(SpriteFont f, SpriteFont sf, HoleManager h, PlayerManager p, PowerUpManager pu,
            Board bo)
        {
            font = f;
            scoreFont = sf;
            holeManager = h;
            playerManager = p;
            powerupManager = pu;
            board = bo;
            gamestate = GameState.normal;
            folds = 0;
            level = 1;
            endLevel = 2;
        }

        public void loadCurrLevel() 
        {
            folds = 0;
            playerManager.restartLevel();
            playerManager.initLevel(XMLReader.Get(level, "player"));
            holeManager.restartLevel();
            holeManager.initLevel(XMLReader.Get(level, "holes"));
            powerupManager.restartLevel();
            powerupManager.initLevel(XMLReader.Get(level, "powerups"));
            board.initLevel(XMLReader.Get(level, "board"));
            Game1.camera.Initialize();
            
        } 

        #region Update
        public void Update(GameTime gameTime)
        {
            if (gamestate != GameState.scored)
            {
                playerManager.Update(gameTime, gamestate);
                boardstate = board.update();
                if (boardstate == Board.BoardState.folding1 || boardstate == Board.BoardState.folding2)
                    gamestate = GameState.folding;
                else if (gamestate != GameState.scored)
                    gamestate = GameState.normal;
                Game1.input.Update(gameTime);
                Game1.camera.UpdateCamera(gameTime);
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    folds = 0;
                    loadCurrLevel();

                }
                if (gamestate == GameState.folding)
                {
                    Vector3 v = board.getAxis();
                    Vector3 p = board.getAxisPoint();
                    float a = board.getAngle();
                    playerManager.foldData(v, p, a, board);

                    holeManager.foldData(v, p, a, board);
                    powerupManager.foldData(v, p, a, board);
                    if (first == 1)
                    {
                        folds++;
                        first = 0;
                    }
                }
                else
                {
                    first = 1;
                    powerupManager.setDrawInFold();
                    holeManager.setDrawInFold();
                }

            }
            if ((gamestate == GameState.scored) && (Mouse.GetState().LeftButton == ButtonState.Pressed))
            {
                gamestate = GameState.normal;
                folds = 0;
                level++;
                if (level <= endLevel)
                    loadCurrLevel();
            }
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            Game1.device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            RasterizerState rs = new RasterizerState();
            // rs.CullMode = CullMode.None;

          //   rs.FillMode = FillMode.WireFrame;    
            Game1.device.RasterizerState = rs;

            board.Draw();
            holeManager.Draw();                        
            powerupManager.Draw();
            playerManager.Draw();
            board.DrawfoldPart();
            holeManager.DrawInFold();
            powerupManager.DrawInFold();
            playerManager.Draw();

            if (gamestate == GameState.scored)
            {
                spriteBatch.DrawString(font,win + folds.ToString() +" folds!", new Vector2(350, 250), Color.Black);
            }

            //spriteBatch.DrawString(font, "Fold the page, till the ink-stain is in the hole", new Vector2(50, 15), Color.Black);
            //spriteBatch.DrawString(font, "Mouse Left Button - choose, Mouse Right Button - cancel", new Vector2(50, graphics.PreferredBackBufferHeight - 50), Color.Black);
            //spriteBatch.DrawString(font, "folds: " + folds, new Vector2(graphics.PreferredBackBufferWidth - 150, 15), Color.Black);
            //spriteBatch.DrawString(font, "level: " + level, new Vector2(graphics.PreferredBackBufferWidth - 150, graphics.PreferredBackBufferHeight - 50), Color.Black);
            //spriteBatch.DrawString(font, "press R to restart level", new Vector2(50, 150), Color.Black
            //        , (MathHelper.Pi / 2) + 0.02f, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font, "Click on the page edges to fold it", new Vector2(1185, 100), Color.Black
            //        , (MathHelper.Pi / 2), new Vector2(0, 0), 1, SpriteEffects.None, 0);
            //if (gamestate == GameState.scored)
            //{
            //    string output = "    WINNER!! \n only " + folds + " folds";
            //    Vector2 FontOrigin = scoreFont.MeasureString(output) / 2;
            //    spriteBatch.DrawString(scoreFont, output, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.Black
            //        , 0, FontOrigin, 1.0f, SpriteEffects.None, 0);
          //  }
        }
        #endregion

        #region Win

        public static void winLevel()
        {
            gamestate = GameState.scored;
        }
        #endregion
    }
}
