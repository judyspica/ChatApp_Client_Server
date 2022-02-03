// -----------------------------------------------------------------------------
// <copyright file="Program.cs" company="FHWN">
// Copyright (c) FHWN. All rights reserved.
// </copyright>
// <summary>This program the fourth homework in programming class.</summary>
// <author>Judy Kardouh.</author>
// -----------------------------------------------------------------------------
namespace ChatApp4th
{
    using System;
    using ChatApp4th.ClientApp;
    using ChatApp4th.ServerApp;

    /// <summary>
    /// The program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method, starting point of program.
        /// </summary>
        public static void Main()
        {
            int initialConsoleWidth = Console.WindowWidth;
            int initialConsoleHeight = Console.WindowHeight;

            ApplicationMode systemMode = SelectMode(initialConsoleWidth, initialConsoleHeight);
            RunMode(systemMode);
        }

        /// <summary>
        /// Allows user to select mode.
        /// </summary>
        /// <param name="initialConsoleWidth">The initial console width.</param>
        /// <param name="initialConsoleHeight">The initial console height.</param>
        /// <returns>User selected integer for further processing.</returns>
        private static ApplicationMode SelectMode(int initialConsoleWidth, int initialConsoleHeight)
        {
            bool validInput = false;
            int selectedMode;
            do
            {
                DisplayTextualOptions();

                string input = Console.ReadLine();
                ConsoleSize.CheckConsoleSize(120, 30);
                bool parseSuccessful = int.TryParse(input, out selectedMode);

                if (parseSuccessful && (selectedMode == 1 || selectedMode == 2 || selectedMode == 3))
                {
                    validInput = true;
                }
            } 
            while (!validInput);

            return ProcessUserSelectedInt(initialConsoleWidth, initialConsoleHeight, selectedMode);
        }

        /// <summary>
        /// Processes the user selected option.
        /// </summary>
        /// <param name="initialConsoleWidth">The initial console width.</param>
        /// <param name="initialConsoleHeight">The initial console height.</param>
        /// <param name="selectedMode">The selected mode.</param>
        /// <returns>The app mode.</returns>
        private static ApplicationMode ProcessUserSelectedInt(int initialConsoleWidth, int initialConsoleHeight, int selectedMode)
        {
            if (selectedMode == 3)
            {
                ConsoleSize.CheckConsoleSize(initialConsoleWidth, initialConsoleHeight);
                Environment.Exit(0);
            }

            if (selectedMode == 1)
            {
                return ApplicationMode.Server;
            }
            else
            {
                return ApplicationMode.Client;
            }
        }

        /// <summary>
        /// Just the textual options.
        /// </summary>
        private static void DisplayTextualOptions()
        {
            Console.WriteLine("Select system mode (enter 1 or 2):");
            Console.WriteLine("1) Server");
            Console.WriteLine("2) Client");
            Console.WriteLine("3) Exit Application");
        }

        /// <summary>
        /// Run mode to start the application accordingly.
        /// </summary>
        /// <param name="systemMode">The application mode to start with.</param>
        private static void RunMode(ApplicationMode systemMode)
        {
            Console.Clear();

            if (systemMode == ApplicationMode.Server)
            {
                Server server = new Server();
                server.IntiServer();
            }
            else
            {
                Client client = new Client();
                client.InitClient();
                client.StartUserInput();
            }
        }
    }
}
