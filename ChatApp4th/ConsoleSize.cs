namespace ChatApp4th
{
    using System;

    public class ConsoleSize
    {
        public static void CheckConsoleSize(int consoleInitialWidth, int consoleInitialHeight)
        {
            if (Console.WindowWidth < consoleInitialWidth && Console.WindowHeight < consoleInitialHeight)
            {
                ResetConsoleSizeToInitial(consoleInitialWidth, consoleInitialHeight);
            }
        }

        public static void ResetConsoleSizeToInitial(int consoleInitialWidth, int consoleInitialHeight)
        {
            Console.SetWindowSize(consoleInitialWidth, consoleInitialHeight);
        }
    }
}
