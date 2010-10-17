#define FakeAccelerometer

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Devices.Sensors;
using System.Net;

namespace WindowsPhonePlatformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformerGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Global content.
        private SpriteFont hudFont;

        private Texture2D winOverlay;
        private Texture2D loseOverlay;
        private Texture2D diedOverlay;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;
        private bool wasContinuePressed;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        #if WINDOWS_PHONE
                private const int TargetFrameRate = 30;        
                private const int BackBufferWidth = 480;
                private const int BackBufferHeight = 800;
                private const Buttons ContinueButton = Buttons.B;        
        #else
                private const int TargetFrameRate = 60;
                private const int BackBufferWidth = 1280;
                private const int BackBufferHeight = 720;
                private const Buttons ContinueButton = Buttons.A;
        #endif

        //AccelerometerState accelState;
        TouchCollection touchState;

        public PlatformerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            // Load fonts
            hudFont = Content.Load<SpriteFont>("Fonts/Hud");

            // Load overlay textures
            winOverlay = Content.Load<Texture2D>("Overlays/you_win");
            loseOverlay = Content.Load<Texture2D>("Overlays/you_lose");
            diedOverlay = Content.Load<Texture2D>("Overlays/you_died");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Content.Load<Song>("Sounds/Music"));

            LoadNextLevel();
        }

        private void LoadNextLevel()
        {
            // Find the path of the next level.
            string levelPath;
            bool fileFound = false;

            // Loop here so we can try again when we can't find a level.
            while (true)
            {
                // Try to find the next level. They are sequentially numbered txt files.
                levelPath = String.Format("Levels/{0}.txt", ++levelIndex);
                levelPath = "Content/" + levelPath;

                try
                {
                    StreamReader sr = new StreamReader(TitleContainer.OpenStream(levelPath));
                    fileFound = true;
                }
                catch
                {
                    fileFound = false;
                }

                if (fileFound)
                    break;

                // If there isn't even a level 0, something has gone wrong.
                if (levelIndex == 0)
                    throw new Exception("No levels found.");

                // Whenever we can't find a level, start over again at 0.
                levelIndex = -1;
            }

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            level = new Level(Services, levelPath);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
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
            HandleInput();

            level.Update(gameTime,  touchState);

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            bool buttonTouched = false;



           
            touchState = TouchPanel.GetState();
            

            //interpert touch screen presses
            foreach (TouchLocation location in touchState)
            {
                switch (location.State)
                {
                    case TouchLocationState.Pressed:
                        buttonTouched = true;
                        break;
                    case TouchLocationState.Moved:
                        break;
                    case TouchLocationState.Released:
                        break;
                }
            }

            // Exit the game when back is pressed.
            if (gamepadState.Buttons.Back == ButtonState.Pressed)
                Exit();

            bool continuePressed =
                    keyboardState.IsKeyDown(Keys.Space) ||
                    gamepadState.IsButtonDown(ContinueButton) || buttonTouched;

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (!wasContinuePressed && continuePressed)
            {
                if (!level.Player.IsAlive)
                {
                    level.StartNewLife();
                }
                else if (level.TimeRemaining == TimeSpan.Zero)
                {
                    if (level.ReachedExit)
                        LoadNextLevel();
                    else
                        ReloadCurrentLevel();
                }
            }

            wasContinuePressed = continuePressed;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            level.Draw(gameTime, spriteBatch);

            DrawHud();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawHud()
        {
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            string timeString = "TIME: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
            Color timeColor;
            if (level.TimeRemaining > WarningTime ||
                level.ReachedExit ||
                (int)level.TimeRemaining.TotalSeconds % 2 == 0)
            {
                timeColor = Color.Yellow;
            }
            else
            {
                timeColor = Color.Red;
            }
            DrawShadowedString(hudFont, timeString, hudLocation, timeColor);

            // Draw score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            DrawShadowedString(hudFont, "SCORE: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.Yellow);

            // Determine the status overlay message to show.
            Texture2D status = null;
            if (level.TimeRemaining == TimeSpan.Zero)
            {
                if (level.ReachedExit)
                {
                    status = winOverlay;
                }
                else
                {
                    status = loseOverlay;
                }
            }
            else if (!level.Player.IsAlive)
            {
                status = diedOverlay;
            }

            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }

    }

//    public class AccelerometerState
//    {
//        public Vector3 Acceleration;
//#if FakeAccelerometer
//        public bool IsJumping; 
//#endif

//        public AccelerometerState()
//        {
//        }
//    }
//#if FakeAccelerometer

//    public class Accelerometer
//    {
//        static string url = "http://davrous8go:8080";

//        static bool pending = false;

//        public static bool Update()
//        {
//            if (pending) return false;
//            pending = true;
//            Uri uri = new Uri(url);

//            WebClient client = new WebClient();
//            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
//            client.DownloadStringAsync(uri);
//            return true;
//        }

//        static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
//        {
//            if (e.Error == null)
//            {
//                string[] numbers = e.Result.Split(new char[] { ';' });
//                state.Acceleration.X = float.Parse(numbers[0]);
//                state.Acceleration.Y = float.Parse(numbers[1]);
//                state.Acceleration.Z = float.Parse(numbers[2]);
//                state.IsJumping = bool.Parse(numbers[3]);
//            }
//            pending = false;
//        }

//        static AccelerometerState state = new AccelerometerState();

//        public static AccelerometerState GetState()
//        {
//            return state;
//        }
//    }

//#else
//    public class Accelerometer
//    {
//        static AccelerometerSensor acceleroSensor;

//        static Accelerometer() 
//        {
//            acceleroSensor = new AccelerometerSensor();
//            acceleroSensor.ReadingChanged += new EventHandler<AccelerometerReadingAsyncEventArgs>(Default_ReadingChanged); 
//        }

//        static AccelerometerState state = new AccelerometerState();

//        static void Default_ReadingChanged(object sender,
//                                          AccelerometerReadingAsyncEventArgs e)
//        {
//            state.Acceleration.X = (float)e.Value.Value.X;
//            state.Acceleration.Y = (float)e.Value.Value.Y;
//            state.Acceleration.Z = (float)e.Value.Value.Z;
//        }

//        public static AccelerometerState GetState()
//        {
//            return state;
//        }
//    }
//    #endif
}
