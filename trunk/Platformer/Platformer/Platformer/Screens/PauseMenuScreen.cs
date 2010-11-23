#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml.Serialization;
using Platformer.SaveGame;
#endregion

namespace Platformer
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        // public readonly XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Pausa")
        {
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Voltar ao jogo");
            MenuEntry saveGameMenuEntry = new MenuEntry("Salvar o jogo");
            MenuEntry quitGameMenuEntry = new MenuEntry("Sair");

            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            saveGameMenuEntry.Selected += SalvarGameMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(saveGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //const string message = "Você tem certeza que deseja sair do jogo?";

            //MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            //confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            //ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);

            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                         new MainMenuScreen());
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void SalvarGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {

            SaveGame();

        }
        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {

            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            base.Draw(gameTime);
        }


        #endregion


        private void SaveGame()
        {
            // serialize out some XML data
            try
            {
                // if (Global.SaveDevice.FileExists(Global.containerName, Global.fileName_options))
                {
                    // save a file asynchronously. this will trigger IsBusy to return true
                    // for the duration of the save process.
                    Global.SaveDevice.Save(
                        Global.containerName,
                        Global.fileName_options,
                        stream =>
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                writer.WriteLine(Global.Lives);
                                writer.WriteLine(Global.Score);
                                writer.WriteLine(Global.ActualLevel);
                            }
                        });
                }

            }
            catch
            {
            }
        }


    }
}
