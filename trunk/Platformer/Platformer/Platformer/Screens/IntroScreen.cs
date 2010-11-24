using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EasyStorage;
using Microsoft.Xna.Framework.Input.Touch;
using Platformer.SaveGame;
using Microsoft.Xna.Framework.Content;
using Microsoft.Phone.Tasks;
using System;
using System.IO.IsolatedStorage;
using System.IO;


namespace Platformer
{
    class IntroScreen : MenuScreen
    {
        private Texture2D image1;
        private Texture2D image2;
        private Texture2D image3;
        private Texture2D ActualImage;
        private ContentManager content;

        private float storedTimer = 0;


        //Video vid;
        private Vector2 ScreenSize
        {
            get
            {
                return new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            }
        }
        public override void LoadContent()
        {
            base.LoadContent();

            if (content == null)
                content = ScreenManager.Game.Content;
            Vector2 position = new Vector2(0, 0);

            this.image1 = content.Load<Texture2D>("Scenes/Intro");// new CustomImage(content.Load<Texture2D>("Scenes/Intro"), position, this.imageSize / 2, this.storedTimer);
            // position.X += this.ScreenSize.X / 2;

            this.image2 = content.Load<Texture2D>("Scenes/Intro2");// new CustomImage(content.Load<Texture2D>("Scenes/Intro2"), position, this.imageSize / 2, this.storedTimer);
            this.image3 = content.Load<Texture2D>("Scenes/Intro3");
        }

        public IntroScreen()
            : base("")
        {


            //MenuEntry startMenuEntry = new MenuEntry("Iniciar");
            //MenuEntry space = new MenuEntry("");

            //// Add entries to the menu.

            //MenuEntries.Add(space);

            //startMenuEntry.Selected += StartMenuEntrySelected;

            //MenuEntries.Add(space);
            //MenuEntries.Add(space);
            //MenuEntries.Add(space);
            //MenuEntries.Add(startMenuEntry);
        }

        void StartMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(ActualImage, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();

            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {

                ActualImage = image1;
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                storedTimer += elapsed;
                if (storedTimer < 7)
                    ActualImage = image1;
                else
                    if (storedTimer >= 7 && storedTimer < 14)
                        ActualImage = image2;
                    else
                        if (storedTimer >= 14 && storedTimer < 21)
                            ActualImage = image3;
                        else
                        {
                            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                                                   new MainMenuScreen());
                        }
            }
        }



    }
}