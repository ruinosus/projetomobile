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
            graphics.ApplyChanges();
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
            PromptMe();
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
              IAsyncSaveDevice saveDevice = new IsolatedStorageSaveDevice();
            Global.SaveDevice = saveDevice;

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
