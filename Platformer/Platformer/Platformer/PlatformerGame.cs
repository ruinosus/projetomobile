//#region File Description
////-----------------------------------------------------------------------------
//// PlatformerGame.cs
////
//// Microsoft XNA Community Game Platform
//// Copyright (C) Microsoft Corporation. All rights reserved.
////-----------------------------------------------------------------------------
//#endregion

//using System;
//using System.IO;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Input.Touch;


//namespace Platformer
//{
//    /// <summary>
//    /// This is the main type for your game
//    /// </summary>
//    public class PlatformerGame : Microsoft.Xna.Framework.Game
//    {
//        // Resources for drawing.
//        private GraphicsDeviceManager graphics;
//        private SpriteBatch spriteBatch;

//        // Global content.
//        private SpriteFont hudFont;

//        private Texture2D winOverlay;
//        private Texture2D loseOverlay;
//        private Texture2D diedOverlay;

//        // Meta-level game state.
//        private int levelIndex = -1;
//        private Level level;
//        private bool wasContinuePressed;

//        // When the time remaining is less than the warning time, it blinks on the hud
//        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

//        // We store our input states so that we only poll once per frame, 
//        // then we use the same input state wherever needed
//        private GamePadState gamePadState;
//        private KeyboardState keyboardState;
//        private TouchCollection touchState;
//        private AccelerometerState accelerometerState;
        
//        // The number of levels in the Levels directory of our content. We assume that
//        // levels in our content are 0-based and that all numbers under this constant
//        // have a level file present. This allows us to not need to check for the file
//        // or handle exceptions, both of which can add unnecessary time to level loading.
//        private const int numberOfLevels = 3;

//        public PlatformerGame()
//        {
//            graphics = new GraphicsDeviceManager(this);
//            Content.RootDirectory = "Content";

//#if WINDOWS_PHONE
//            graphics.IsFullScreen = true;
//            TargetElapsedTime = TimeSpan.FromTicks(333333);
//#endif

//            Accelerometer.Initialize();
//        }

//        /// <summary>
//        /// LoadContent will be called once per game and is the place to load
//        /// all of your content.
//        /// </summary>
//        protected override void LoadContent()
//        {
//            // Create a new SpriteBatch, which can be used to draw textures.
//            spriteBatch = new SpriteBatch(GraphicsDevice);

//            // Load fonts
//            hudFont = Content.Load<SpriteFont>("Fonts/Hud");

//            // Load overlay textures
//            winOverlay = Content.Load<Texture2D>("Overlays/you_win");
//            loseOverlay = Content.Load<Texture2D>("Overlays/you_lose");
//            diedOverlay = Content.Load<Texture2D>("Overlays/you_died");

//            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
//            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
//            //Which means its impossible to test this from VS.
//            //So we have to catch the exception and throw it away
//            try
//            {
//                MediaPlayer.IsRepeating = true;
//                MediaPlayer.Play(Content.Load<Song>("Sounds/Music"));
//            }
//            catch { }

//            LoadNextLevel();
//        }

//        /// <summary>
//        /// Allows the game to run logic such as updating the world,
//        /// checking for collisions, gathering input, and playing audio.
//        /// </summary>
//        /// <param name="gameTime">Provides a snapshot of timing values.</param>
//        protected override void Update(GameTime gameTime)
//        {
//            // Handle polling for our input and handling high-level input
//            HandleInput();

//            // update our level, passing down the GameTime along with all of our input states
//            level.Update(gameTime, keyboardState, gamePadState, touchState, 
//                         accelerometerState, Window.CurrentOrientation);

//            base.Update(gameTime);
//        }

//        private void HandleInput()
//        {
//            // get all of our input states
//            keyboardState = Keyboard.GetState();
//            gamePadState = GamePad.GetState(PlayerIndex.One);
//            touchState = TouchPanel.GetState();
//            accelerometerState = Accelerometer.GetState();

//            // Exit the game when back is pressed.
//            if (gamePadState.Buttons.Back == ButtonState.Pressed)
//                Exit();

//            bool continuePressed =
//                keyboardState.IsKeyDown(Keys.Space) ||
//                gamePadState.IsButtonDown(Buttons.A) ||
//                touchState.AnyTouch();

//            // Perform the appropriate action to advance the game and
//            // to get the player back to playing.
//            if (!wasContinuePressed && continuePressed)
//            {
//                if (!level.Player.IsAlive)
//                {
//                    level.StartNewLife();
//                }
//                else if (level.TimeRemaining == TimeSpan.Zero)
//                {
//                    if (level.ReachedExit)
//                        LoadNextLevel();
//                    else
//                        ReloadCurrentLevel();
//                }
//            }

//            wasContinuePressed = continuePressed;
//        }

//        private void LoadNextLevel()
//        {
//            // move to the next level
//            levelIndex = (levelIndex + 1) % numberOfLevels;

//            // Unloads the content for the current level before loading the next one.
//            if (level != null)
//                level.Dispose();

//            // Load the level.
//            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
//            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
//                level = new Level(Services, fileStream, levelIndex);
//        }

//        private void ReloadCurrentLevel()
//        {
//            --levelIndex;
//            LoadNextLevel();
//        }

//        /// <summary>
//        /// Draws the game from background to foreground.
//        /// </summary>
//        /// <param name="gameTime">Provides a snapshot of timing values.</param>
//        protected override void Draw(GameTime gameTime)
//        {
//            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);


//            spriteBatch.Begin();

//            level.Draw(gameTime, spriteBatch);

//            DrawHud();

//            spriteBatch.End();

//            base.Draw(gameTime);
//        }

//        private void DrawHud()
//        {
//            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
//            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
//            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
//                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

//            // Draw time remaining. Uses modulo division to cause blinking when the
//            // player is running out of time.
//            string timeString = "TIME: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
//            Color timeColor;
//            if (level.TimeRemaining > WarningTime ||
//                level.ReachedExit ||
//                (int)level.TimeRemaining.TotalSeconds % 2 == 0)
//            {
//                timeColor = Color.Yellow;
//            }
//            else
//            {
//                timeColor = Color.Red;
//            }
//            DrawShadowedString(hudFont, timeString, hudLocation, timeColor);

//            // Draw score
//            float timeHeight = hudFont.MeasureString(timeString).Y;
//            DrawShadowedString(hudFont, "SCORE: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.Yellow);
           
//            // Determine the status overlay message to show.
//            Texture2D status = null;
//            if (level.TimeRemaining == TimeSpan.Zero)
//            {
//                if (level.ReachedExit)
//                {
//                    status = winOverlay;
//                }
//                else
//                {
//                    status = loseOverlay;
//                }
//            }
//            else if (!level.Player.IsAlive)
//            {
//                status = diedOverlay;
//            }

//            if (status != null)
//            {
//                // Draw status message.
//                Vector2 statusSize = new Vector2(status.Width, status.Height);
//                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
//            }
//        }

//        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
//        {
//            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
//            spriteBatch.DrawString(font, value, position, color);
//        }
//    }
//}

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;
using EasyStorage;
using Platformer.SaveGame;

namespace Platformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformerGame : Microsoft.Xna.Framework.Game
    {
        // Resources for drawing.
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        ScreenManager screenManager;







#if ZUNE
        private const int TargetFrameRate = 30;        
        private const int BackBufferWidth = 240;
        private const int BackBufferHeight = 320;
        private const Buttons ContinueButton = Buttons.B;        
#else
        private const int TargetFrameRate = 60;
        private const int BackBufferWidth = 1280;
        private const int BackBufferHeight = 720;
        private const Buttons ContinueButton = Buttons.A;
#endif

        public PlatformerGame()
        {
            this.IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;

            Content.RootDirectory = "Content";

            // Framerate differs between platforms.
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            //screenManager.AddScreen(new PressStartScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            // Load fonts
            // sceneManager = new SceneManager(this, Services, graphics, GraphicsDevice);

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // sceneManager.Update(gameTime);
            base.Update(gameTime);
        }



        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // spriteBatch.Begin();
            //  sceneManager.Draw(gameTime);
            // spriteBatch.End();
            base.Draw(gameTime);
            
        }

        protected override void Initialize()
        {
             TouchPanel.EnabledGestures =
              GestureType.Hold |
              GestureType.Tap |
              GestureType.DoubleTap |
              GestureType.FreeDrag |
              GestureType.Flick |
              GestureType.Pinch;
          //   PromptMe();
            base.Initialize();
        }

        private void PromptMe()
        {
            // we can set our supported languages explicitly or we can allow the
            // game to support all the languages. the first language given will
            // be the default if the current language is not one of the supported
            // languages. this only affects the text found in message boxes shown
            // by EasyStorage and does not have any affect on the rest of the game.
            EasyStorageSettings.SetSupportedLanguages(Language.French, Language.Spanish);

            // on Windows Phone we use a save device that uses IsolatedStorage
            // on Windows and Xbox 360, we use a save device that gets a 
            //shared StorageDevice to handle our file IO.
#if WINDOWS_PHONE

            Global.SaveDevice = new IsolatedStorageSaveDevice(); ;

            // we use the tap gesture for input on the phone
            TouchPanel.EnabledGestures = GestureType.Tap;
#else
            // create and add our SaveDevice
            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
            ScreenManager.Game.Components.Add(sharedSaveDevice);

            // make sure we hold on to the device
            saveDevice = sharedSaveDevice;

            // hook two event handlers to force the user to choose a new device if they cancel the
            // device selector or if they disconnect the storage device after selecting it
            sharedSaveDevice.DeviceSelectorCanceled += 
                (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceDisconnected += 
                (s, e) => e.Response = SaveDeviceEventResponse.Force;

            // prompt for a device on the first Update we can
            sharedSaveDevice.PromptForDevice();

            sharedSaveDevice.DeviceSelected += (s, e) =>
            {
                //Save our save device to the global counterpart, so we can access it
                //anywhere we want to save/load
                Global.SaveDevice = (SaveDevice)s;

                //Once they select a storage device, we can load the main menu.
                //You'll notice I hard coded PlayerIndex.One here. You'll need to 
                //change that if you plan on releasing your game. I linked to an
                //example on how to do that but here's the link if you need it.
                //http://blog.nickgravelyn.com/2009/03/basic-handling-of-multiple-controllers/
                ScreenManager.AddScreen(new MainMenuScreen(), PlayerIndex.One);
            };
#endif

#if XBOX
            // add the GamerServicesComponent
            ScreenManager.Game.Components.Add(
                new Microsoft.Xna.Framework.GamerServices.GamerServicesComponent(ScreenManager.Game));
#endif
        }


    }
}
