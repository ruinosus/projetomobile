using System;

namespace WindowsPhonePlatformer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PlatformerGamer game = new PlatformerGamer())
            {
                game.Run();
            }
        }
    }
#endif
}

