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
    class PressStartScreen : MenuScreen
    {

        //Video vid;
      

        public PressStartScreen()
            : base("")
        {
            //ContentManager content = new VideoContentManager(ScreenManager.Game.Services);
            //vid= content.Load<Video>("Videos/intro");
            //vid.Loop = true; //or false
            //vid.Play();

            MenuEntry startMenuEntry = new MenuEntry("Iniciar");
            MenuEntry space = new MenuEntry("");
          
            // Add entries to the menu.

            MenuEntries.Add(space);

            startMenuEntry.Selected += StartMenuEntrySelected;

            MenuEntries.Add(space);
            MenuEntries.Add(space);
            MenuEntries.Add(space);
            MenuEntries.Add(startMenuEntry);
        }

        //public override void LoadContent()
        //{
        //    base.LoadContent();
        //    MediaPlayerLauncher mpl = new MediaPlayerLauncher();
        //    mpl.Controls = MediaPlaybackControls.None;
        //    mpl.Location = MediaLocationType.Install;
        //    mpl.Media = new Uri(@"Content\Videos\intro.wmv", UriKind.Relative);
        //    mpl.Show();
        //}

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

        //public override void Draw(GameTime gameTime)
        //{
        //    // This game has a blue background. Why? Because!
        //    ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
        //                                       Color.CornflowerBlue, 0, 0);
        //    SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

        //    //if (vid.IsPlaying)
        //    //{
        //    //    spriteBatch.Begin();
        //    //    spriteBatch.Draw(vid.CurrentTexture, new Vector2(10, 10), Color.White);
        //    //    spriteBatch.End();
        //    //}
            
        //    if (TransitionPosition > 0)
        //        ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        //}

    
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        //public override void Update(GameTime gameTime, bool otherScreenHasFocus,
        //                                               bool coveredByOtherScreen)
        //{
        //    base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        //    if (IsActive)
        //    {
        //        //vid.Update();
        //    }
        //}
    }
}