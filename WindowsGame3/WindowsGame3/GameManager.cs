using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Microsoft.Xna.Framework.Audio;

namespace Foldit3D
{
    enum GameState { normal, folding, scored , lose};

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
        int endLevel = 2;
        int folds;
        int first = 1;
        int score = 0;
        float time;

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
        }

        public void loadCurrLevel() 
        {
            folds = 0;
            time = 0;
            gamestate = GameState.normal;
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
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            time += elapsed;
            if (time >= 60)
            {
                loseLevel();
            }
            if (gamestate != GameState.scored && gamestate != GameState.lose)
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
            // rs.FillMode = FillMode.WireFrame;    
            Game1.device.RasterizerState = rs;

            board.Draw();
            holeManager.Draw();                        
            powerupManager.Draw();
            playerManager.Draw();
            board.DrawfoldPart();
            holeManager.DrawInFold();
            powerupManager.DrawInFold();
            playerManager.Draw();

            spriteBatch.DrawString(scoreFont, "score: " + score, new Vector2(20, 0), Color.LightGray);
            spriteBatch.DrawString(scoreFont, "folds: " + folds, new Vector2(20, 40), Color.LightGray);
            if (gamestate != GameState.lose)
            { spriteBatch.DrawString(scoreFont, "time: " + Convert.ToString(60 - (int)time), new Vector2(20, graphics.PreferredBackBufferHeight - 50), Color.LightGray); }

            if (gamestate == GameState.scored)
            {
                if (folds < 3) score += 100;
                else if (folds < 5) score += 75;
                else if (folds < 7) score += 50;
                else if (folds < 9) score += 25;
                spriteBatch.DrawString(font,win + folds.ToString() +" folds!", new Vector2(350, 250), Color.Black);
            }
            else if (gamestate == GameState.lose)
            {
                spriteBatch.DrawString(font, "Ooooops! you ran out of time...", new Vector2(250, 250), Color.Black);
                spriteBatch.DrawString(scoreFont, "time: " + 0, new Vector2(20, graphics.PreferredBackBufferHeight - 50), Color.LightGray); 
                if (time > 65) { loadCurrLevel(); }
            }

            if (level==1)
            {
                showPopUps(spriteBatch);
                if (folds == 1) { spriteBatch.DrawString(scoreFont, "Great! You made your first fold!", new Vector2(400, 20), Color.Black); }
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

        #region Win/Lose

        public static void winLevel()
        {
            gamestate = GameState.scored;
        }

        public static void loseLevel()
        {
            gamestate = GameState.lose;
        }
        #endregion

        #region PopUps
        public void showPopUps(SpriteBatch spriteBatch) 
        {
            if (time < 5)
            {
                spriteBatch.DrawString(scoreFont, "This is you... -> \n Put yourself in the hole!", new Vector2(160, 470), Color.Black);
                spriteBatch.DrawString(scoreFont, "<- Hole", new Vector2(760, 210), Color.Black);
                spriteBatch.DrawString(scoreFont, "<- Take me!\n     I'm a surprise!", new Vector2(865, 490), Color.Black);
            }
            else if (time >= 5 && time < 10)
            {
                spriteBatch.DrawString(scoreFont, "           ^ \n Try to click here", new Vector2(400, 600), Color.Black);
                spriteBatch.DrawString(scoreFont, "           ^ \n Try to click here too", new Vector2(430, 80), Color.Black);
            }
                
        }
        #endregion
    }
}
