using System;

namespace GamePadServer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GamePadServer game = new GamePadServer())
            {
                game.Run();
            }
        }
    }
#endif
}

