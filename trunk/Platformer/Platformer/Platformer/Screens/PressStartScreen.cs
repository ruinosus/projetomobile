using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Platformer
{
    class PressStartScreen : MenuScreen
    {
        public PressStartScreen()
            : base("")
        {
            MenuEntry startMenuEntry = new MenuEntry("Precione a tecla A ou Enter para começar");
            startMenuEntry.Selected += StartMenuEntrySelected;
            MenuEntries.Add(startMenuEntry);
        }

        void StartMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
        #if WINDOWS
            Global.SaveDevice = new PCSaveDevice("JogoSalvo" + "_Save");
            ScreenManager.AddScreen(new BackgroundScreen(), e.PlayerIndex);
            ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
        #else
            PromptMe();
        #endif
        }

        private void PromptMe()
        {
            //SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
            //ScreenManager.Game.Components.Add(sharedSaveDevice);

            //// hook an event for when the device is selected
            //sharedSaveDevice.DeviceSelected += (s, e) =>
            //{
            //    Global.SaveDevice = (SaveDevice)s;
            //    ScreenManager.AddScreen(new MainMenuScreen(), PlayerIndex.One);
            //};

            //// hook two event handlers to force the user to choose a new device if they cancel the
            //// device selector or if they disconnect the storage device after selecting it
            //sharedSaveDevice.DeviceSelectorCanceled += 
            //    (s, e) => e.Response = SaveDeviceEventResponse.Force;
            //sharedSaveDevice.DeviceDisconnected += 
            //    (s, e) => e.Response = SaveDeviceEventResponse.Force;

            //// prompt for a device on the next Update
            //sharedSaveDevice.PromptForDevice();

            //// make sure we hold on to the device
            //Global.SaveDevice = sharedSaveDevice;
        }
    }
}