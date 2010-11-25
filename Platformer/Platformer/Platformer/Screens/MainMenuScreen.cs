#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Platformer.SaveGame;
using System.IO;
#endregion

namespace Platformer
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Novo Jogo");
            MenuEntry loadMenuEntry = new MenuEntry("Carregar");
            MenuEntry space = new MenuEntry("");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            loadMenuEntry.Selected += LoadMenuEntrySelected;

            // Add entries to the menu.
          
            MenuEntries.Add(space);       
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(loadMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameIntroScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void LoadMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadGame();
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                              new GameplayScreen());
           
        }


        public void LoadGame()
        {
            if (Global.SaveDevice.FileExists(Global.containerName, Global.fileName_options))
            {
                Global.IsLoaded = true;
                Global.SaveDevice.Load(
                    Global.containerName,
                    Global.fileName_options,
                    stream =>
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            Global.Lives = int.Parse(reader.ReadLine());
                            Global.Score = int.Parse(reader.ReadLine());
                            Global.ActualLevel = int.Parse(reader.ReadLine());
                            //lives = int.Parse(reader.ReadLine());
                            //doIHaveTheKey = bool.Parse(reader.ReadLine());
                            //score = int.Parse(reader.ReadLine());
                        }
                    });
            }
        }

        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }


        #endregion

        
    }
}
