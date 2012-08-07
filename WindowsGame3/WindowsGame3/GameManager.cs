using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WindowsGame3;

namespace Foldit3D
{
enum ScreenState { start, game, help, levels};
enum GameState { normal, folding, scored , lose};

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
    Table table;
    bool prefold;
    ScreenState screen = ScreenState.start;
    Texture2D startScreen;
    Texture2D helpScreen;
    Texture2D levelScreen;
    Rectangle newGameBtn = new Rectangle(400, 200, 400, 70);
    Rectangle levelsBtn = new Rectangle(400, 300, 400, 70);
    Rectangle helpBtn = new Rectangle(400, 400, 400, 70);
    Rectangle helpBackBtn = new Rectangle(800, 570, 200, 70);
    List<Texture2D> levelsPics = new List<Texture2D>();
    List<Rectangle> levelsPicsBtn = new List<Rectangle>();
    double levelScreenTime = 0;
    string win = "      EXCELLENT!!!\n you did it with: ";
    int level;
    int endLevel = 2;
    int folds;
    int first = 1;
    int score = 0;
    static bool changeScore = false;
    float time;
    int firstfold = 0;

    ///////////////////////////
    List<IDictionary<string, string>> levels = new List<IDictionary<string, string>>();


    public GameManager(SpriteFont f, SpriteFont sf, HoleManager h, PlayerManager p, PowerUpManager pu,
        Board bo,Table tab, Texture2D st, Texture2D help, Texture2D ls)
    {
        font = f;
        scoreFont = sf;
        holeManager = h;
        playerManager = p;
        powerupManager = pu;
        board = bo;
        table = tab;
        gamestate = GameState.normal;
        folds = 0;
        level = 0;

        startScreen = st;
        helpScreen = help;
        levelScreen = ls;
        for (int i = 1; i <= endLevel; i++)
        {
            levelsPics.Add(Game1.content.Load<Texture2D>("level" + i));
            levelsPicsBtn.Add(new Rectangle(80 + ((levelsPics.ElementAt(i - 1).Width+30) * i), (i < 4) ? 220 : 400, levelsPics.ElementAt(i - 1).Width, levelsPics.ElementAt(i - 1).Height));
        }
            
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
        //powerupManager.tomInitLevel();
        board.initLevel(XMLReader.Get(level, "board"));
        Game1.camera.Initialize(); 
    } 

    #region Update
    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (screen == ScreenState.start || screen == ScreenState.help|| screen==ScreenState.levels)
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
                        levelScreenTime = 0.1;
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

                else if (screen == ScreenState.levels)
                {
                    if (levelScreenTime > 0) Math.Max(levelScreenTime -= elapsed, 0);
                    if (levelScreenTime > 0) return;
                    if (new Rectangle(mouseState.X, mouseState.Y, 1, 1).Intersects(helpBackBtn))
                    {
                        screen = ScreenState.start;
                    }
                    else
                    {
                        for (int i = 0; i < endLevel; i++)
                        {
                            if (new Rectangle(mouseState.X, mouseState.Y, 1, 1).Intersects(levelsPicsBtn.ElementAt(i)))
                            {
                                level = i + 1;
                                screen = ScreenState.game;
                                loadCurrLevel();
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
               
            time += elapsed;
        if (time >= 60 && level != 0)
        {
            loseLevel();
        }
        if (gamestate != GameState.scored && gamestate != GameState.lose)
        {
            playerManager.Update(gameTime, gamestate);
            boardstate = board.update();
            if (boardstate == Board.BoardState.folding1 || boardstate == Board.BoardState.folding2)
            {
                gamestate = GameState.folding;                    
            }
            else if (gamestate != GameState.scored)
                gamestate = GameState.normal;
            if (boardstate == Board.BoardState.preFold)
                prefold = true;
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
                    
                //Tom -  gets the folding points
                Vector3 foldPoint1, foldPoint2;
                board.getfoldPoints(out foldPoint1,out  foldPoint2);
                    
                if (prefold)
                {
                    powerupManager.preFoldData(foldPoint1, foldPoint2, v, board);
                    holeManager.preFoldData(foldPoint1, foldPoint2, v, board);
                    playerManager.preFoldData(foldPoint1, foldPoint2, v, board);
                    prefold = false;
                }

                playerManager.foldData(a, board);

                holeManager.foldData(a, board);
             
                powerupManager.foldData(a,board);
              
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
        // rs.CullMode = CullMode.None;  // if enabled , drawing only facing objects
            //rs.FillMode = FillMode.WireFrame;  
            
        Game1.device.RasterizerState = rs;

        spriteBatch.Begin();

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
            for (int i = 1; i <= endLevel; i++)
            {
                spriteBatch.Draw(levelsPics.ElementAt(i - 1),levelsPicsBtn.ElementAt(i-1), Color.White);
                var rect = new Texture2D(graphics.GraphicsDevice, 1, 1);
                rect.SetData(new[] { Color.Transparent });
                spriteBatch.Draw(rect, levelsPicsBtn.ElementAt(i - 1), Color.White);
                spriteBatch.DrawString(font, "Back", new Vector2(helpBackBtn.X + 50, helpBackBtn.Y), Color.Black);
                var rect3 = new Texture2D(graphics.GraphicsDevice, 1, 1);
                rect3.SetData(new[] { Color.Transparent });
                spriteBatch.Draw(rect3, helpBackBtn, Color.White);
                
            }
        }
        else
        {
            spriteBatch.End(); //Tom 
            Game1.device.DepthStencilState = DepthStencilState.Default; //TOM - makes 3d 

            table.Draw();
            board.Draw();
            board.DrawfoldPart();
            holeManager.Draw();
            powerupManager.Draw();
            powerupManager.DrawInFold();
            holeManager.DrawInFold();
            playerManager.Draw();


            spriteBatch.Begin(); //Tom 
            spriteBatch.DrawString(scoreFont, "score: " + score, new Vector2(20, 0), Color.LightGray);
            spriteBatch.DrawString(scoreFont, "level: " + level, new Vector2(20, 40), Color.LightGray);
            spriteBatch.DrawString(scoreFont, "folds: " + folds, new Vector2(20, 80), Color.LightGray);
            if (gamestate != GameState.lose && gamestate != GameState.scored && level != 0)
            { spriteBatch.DrawString(scoreFont, "time: " + Convert.ToString(60 - (int)time), new Vector2(20, graphics.PreferredBackBufferHeight - 50), Color.LightGray); }

            if (gamestate == GameState.scored)
            {
                if (!changeScore)
                {
                    if (folds < 3) score += 100;
                    else if (folds < 5) score += 75;
                    else if (folds < 7) score += 50;
                    else if (folds < 9) score += 25;
                    changeScore = true;
                }
                spriteBatch.DrawString(font, win + folds.ToString() + " folds!", new Vector2(350, 250), Color.Black);
                spriteBatch.DrawString(scoreFont, "time: " + 0, new Vector2(20, graphics.PreferredBackBufferHeight - 50), Color.LightGray); 
            }
            else if (gamestate == GameState.lose)
            {
                spriteBatch.DrawString(font, "Ooooops! you ran out of time...", new Vector2(250, 250), Color.Black);
                spriteBatch.DrawString(scoreFont, "time: " + 0, new Vector2(20, graphics.PreferredBackBufferHeight - 50), Color.LightGray);
                if (time > 65) { loadCurrLevel(); }
            }

            if (level == 0)
            {
                showPopUps(spriteBatch);
                if (folds == 1)
                {
                    spriteBatch.DrawString(scoreFont, "Great! You made your first fold!", new Vector2(400, 20), Color.Black);
                    firstfold = 1;
                }
                if (folds == 2)
                {
                    spriteBatch.DrawString(scoreFont, "Amazing! You made your second fold!", new Vector2(330, 10), Color.Black);
                    firstfold = 2;
                    spriteBatch.DrawString(scoreFont, "Go on! click on the paper frame and get out from the hole!", new Vector2(230, 45), Color.Black);
                }
            }
            else if (level == 1 && time < 5)
                spriteBatch.DrawString(scoreFont, "<- Take me!\n     I'm a surprise!", new Vector2(865, 490), Color.Black);
            if (folds > 9 && folds <13)
                spriteBatch.DrawString(scoreFont, "Don't give up! You can do it!", new Vector2(400, 20), Color.Black);
            else if (folds > 13)
                spriteBatch.DrawString(scoreFont, "You can press 'R' to restart...", new Vector2(400, 20), Color.Black);
        }
        spriteBatch.End(); //Tom 
    }
    #endregion

    #region Win/Lose

    public static void winLevel()
    {
        gamestate = GameState.scored;
        changeScore = false;
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
        }
        else if (time >= 5 && firstfold==0)
        {
            spriteBatch.DrawString(scoreFont, "           ^ \n Try to click here", new Vector2(400, 600), Color.Black);
            spriteBatch.DrawString(scoreFont, "           ^ \n Try to click here too", new Vector2(430, 80), Color.Black);
        }
        else if (firstfold==1)
        {
            spriteBatch.DrawString(scoreFont, "Click here   >", new Vector2(830, 400), Color.Black);
            spriteBatch.DrawString(scoreFont, "<   Click here", new Vector2(200, 330), Color.Black);
        }
                
                
    }
    #endregion
}
}
