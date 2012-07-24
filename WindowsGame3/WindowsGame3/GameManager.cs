using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Foldit3D
{
    enum ScreenState { start, game, help, levels};
    enum GameState { normal, folding, scored };
    //public enum BoardState { chooseEdge1, onEdge1, chooseEdge2, onEdge2, preFold, folding1, folding2 };

    class GameManager
    {
        SpriteFont font, scoreFont;
        //Board board;
        static GameState gamestate;
        Board.BoardState boardstate;
        HoleManager holeManager;
        PlayerManager playerManager;
        PowerUpManager powerupManager;
        Board board;
        ScreenState screen = ScreenState.start;
        Texture2D startScreen;
        Texture2D helpScreen;
        Texture2D levelScreen;
        Rectangle newGameBtn = new Rectangle(400, 200, 400, 70);
        Rectangle levelsBtn = new Rectangle(400, 300, 400, 70);
        Rectangle helpBtn = new Rectangle(400, 400, 400, 70);
        Rectangle helpBackBtn = new Rectangle(800, 570, 200, 70);

        string win = "      EXCELLENT!!!\n you did it with: ";
        int level;
        int endLevel;
        int folds;
        int first = 1;

        ///////////////////////////
        List<IDictionary<string, string>> levels = new List<IDictionary<string, string>>();

        public GameManager(SpriteFont f, SpriteFont sf, HoleManager h, PlayerManager p, PowerUpManager pu,
            Board bo, Texture2D st, Texture2D help, Texture2D ls)
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
            endLevel = 1;
            startScreen = st;
            helpScreen = help;
            levelScreen = ls;
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
            Vector3[] points = new Vector3[4] {
                new Vector3(-40f, 0f, 25f),
                new Vector3(40f, 0f, 25f),
                new Vector3(40f, 0f, -25f),
               // new Vector3(0f, 0f, -40f),
                new Vector3(-40f, 0f, -25f)
             };
            Vector2[] texCords = new Vector2[4] {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1),
               // new Vector2(1,1),
                new Vector2(0,1)  
             };
            Game1.camera.Initialize();
            board.Initialize(4, points, texCords);
        } 

        #region Update
        public void Update(GameTime gameTime)
        {
            if (screen == ScreenState.start || screen == ScreenState.help)
            {
                MouseState mouseState = Mouse.GetState();
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (screen == ScreenState.start)
                    {
                        if (new Rectangle(mouseState.X, mouseState.Y, 1, 1).Intersects(newGameBtn))
                        {
                            screen = ScreenState.game;
                        }
                        else if (new Rectangle(mouseState.X, mouseState.Y, 1, 1).Intersects(levelsBtn))
                        {
                            screen = ScreenState.levels;
                        }
                        else if (new Rectangle(mouseState.X, mouseState.Y, 1, 1).Intersects(helpBtn))
                        {
                            screen = ScreenState.help;
                        }
                    }
                    else if (screen == ScreenState.help)
                    {
                        if (new Rectangle(mouseState.X, mouseState.Y, 1, 1).Intersects(helpBackBtn))
                        {
                            screen = ScreenState.start;
                        }
                    }
                }
            }
            else
            {
                if (gamestate != GameState.scored)
                {
                    playerManager.Update(gameTime, gamestate);
                    //gamestate = board.update();
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
                        // if (boardstate == Board.BoardState.folding1)
                        playerManager.foldData(v, p, a, board);
                        //  if (boardstate == Board.BoardState.folding2)
                        //      playerManager.foldDataAfter(v, p, a, board);

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

            if (screen == ScreenState.start)
            {
                spriteBatch.Draw(startScreen, new Rectangle(210, 60, startScreen.Width, startScreen.Height), Color.White);
                //Start Game btn
                spriteBatch.DrawString(font, "Start Game", new Vector2(newGameBtn.X + 60, newGameBtn.Y), Color.Black);
                var rect = new Texture2D(graphics.GraphicsDevice, 1, 1);
                rect.SetData(new[] { Color.Transparent });
                spriteBatch.Draw(rect, newGameBtn, Color.White);
                

                //Levels btn
                spriteBatch.DrawString(font, "Levels", new Vector2(levelsBtn.X + 127, levelsBtn.Y), Color.Black);
                var rect1 = new Texture2D(graphics.GraphicsDevice, 1, 1);
                rect1.SetData(new[] { Color.Transparent });
                spriteBatch.Draw(rect1, levelsBtn, Color.White);
                
                //Help btn
                spriteBatch.DrawString(font, "Help", new Vector2(helpBtn.X + 150, helpBtn.Y), Color.Black);
                var rect2 = new Texture2D(graphics.GraphicsDevice, 1, 1);
                rect2.SetData(new[] { Color.Transparent });
                spriteBatch.Draw(rect2, helpBtn, Color.White);

            }
            else if (screen == ScreenState.help)
            {
                spriteBatch.Draw(helpScreen, new Rectangle(210, 60, helpScreen.Width, helpScreen.Height), Color.White);
                spriteBatch.DrawString(font, "Back", new Vector2(helpBackBtn.X + 50, helpBackBtn.Y), Color.Black);
                spriteBatch.DrawString(scoreFont, "Your goal is to free the gum stuck on paper by moving\n it to the hole. The only way to move the gum is by\n folding the paper and sticking it to the other side.\nCan you do it with minimum number of fold?\n\nKeys:\nFold the paper by clicking on 2 points on different edges.\nUse the arrows keys to rotate the papaer,\n X to zoom in and Z to zoom out.", new Vector2(260, 160), Color.Black);
                //back btn
                var rect3 = new Texture2D(graphics.GraphicsDevice, 1, 1);
                rect3.SetData(new[] { Color.Transparent });
                spriteBatch.Draw(rect3, helpBackBtn, Color.White);  
            }
            else if (screen == ScreenState.levels)
            {
                spriteBatch.Draw(levelScreen, new Rectangle(210, 60, levelScreen.Width, levelScreen.Height), Color.White);
            }
            else
            {
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
                    spriteBatch.DrawString(font, win + folds.ToString() + " folds!", new Vector2(350, 250), Color.Black);
                }
            }
        }
        #endregion

        #region Win
        public static void winLevel()
        {
            gamestate = GameState.scored;
            // print to screen 
        }
        #endregion
    }
}
