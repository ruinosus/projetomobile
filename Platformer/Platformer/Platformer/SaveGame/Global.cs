using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyStorage;

namespace Platformer.SaveGame
{
    public class Global
    {
        // A generic EasyStorage save device 
        public static IAsyncSaveDevice SaveDevice;

        //We can set up different file names for different things we may save.
        //In this example we're going to save the items in the 'Options' menu.
        //I listed some other examples below but commented them out since we
        //don't need them. YOU CAN HAVE MULTIPLE OF THESE
        public static string fileName_options = "Platformer_Options";
        //public static string fileName_game = "YourGame_Game";
        //public static string fileName_awards = "YourGame_Awards";

        //This is the name of the save file you'll find if you go into your memory
        //options on the Xbox. If you name it something like 'MyGameSave' then
        //people will have no idea what it's for and might delete your save.
        //YOU SHOULD ONLY HAVE ONE OF THESE
        public static string containerName = "Platformer_Save";

        public static int Score;
        public static int Lives;
        public static int ActualLevel;
        public static bool IsLoaded;

        /*
         
         . get the latest version: http://www.codeplex.com/ScurvyMedia/Release/ProjectReleases.aspx
2. Add a Scurvy.Media.Pipeline reference to the project's nested content project
3. Add a reference from your game, to Scurvy.Media.dll
4. Add your .avi video to your XNA Game Studio 3.0 project, it should default to the scurvy media importer and processor.
5. Important In your game, declare a new VideoContentManager, and load your video using that:
ContentManager content = new VideoContentManager(Services);
Video vid = content.Load<Video>("myVideo");
vid.Loop = true; //or false
vid.Play();

6. Call the video's update method in the game's update method:
vid.Update();

7. In the game's render method, render the video's current texture using whatever means you choose. This example will let you render it using a sprite batch:
if (vid.IsPlaying)
{
  batch.Begin();
  batch.Draw(vid.CurrentTexture, new Vector2(10, 10), Color.White);
  batch.End();
}

         */

    }
}
