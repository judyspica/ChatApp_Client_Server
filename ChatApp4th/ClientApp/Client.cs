namespace ChatApp4th.ClientApp
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using ChatApp4th.ServerApp;

    public class Client
    {
        private TcpClient tcpClient;
        private TcpMessagesThreadLinstner tcpThreadLinstner;
        private ClientDetails clientDetails;
        private bool clinetRunning;

        private NetworkStream NetworkStream
        {
            get
            {
                return this.tcpClient.GetStream();
            }
        }

        public void InitClient()
        {
            this.tcpClient = this.GetValidServerConnection();
            this.tcpThreadLinstner = new TcpMessagesThreadLinstner(this.tcpClient);
            this.tcpThreadLinstner.CommandReceived += this.ProcessCommand;
            this.AskForNickName();
            this.clinetRunning = true;
        }

        public void StartUserInput()
        {
            DisplayWelcomeMessage();

            try
            {
                while (this.clinetRunning)
                {
                    // Delete the line previous.
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    this.WriteToServer(Console.ReadLine());
                }
            }
            catch (Exception)
            {
                Console.WriteLine("unable to send messages currently, press any key to exit console.");
                Console.ReadLine();
            }
        }

        private static void DisplayWelcomeMessage()
        {
            ConsoleSize.CheckConsoleSize(120, 30);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Welcome :) you can now start chatting!\n\nKeep in mind that you can change your name and color by typing:\n- changename somename\n- changecolor somecolor\n  you can also write exit to disconnect from the chat.\n\n");
            Console.ResetColor();
        }

        private static string GetTimenow()
        {
            DateTime now = DateTime.Now;
            string timeNow = now.ToString("HH:mm:ss ");
            return timeNow;
        }

        private void AskForNickName()
        {
            this.clientDetails = ClientDetails.InitWithDefaults();

            Console.Write("Enter new nickname: ");

            string newNickName = Console.ReadLine();

            this.clientDetails.Nickname = newNickName;
            this.WriteToServer("nickname " + newNickName);
        }

        private void WriteToServer(string message)
        {
            string formatedInput = this.ConvertToFormatedInput(message);
            byte[] buffer = Encoding.ASCII.GetBytes(formatedInput);
            NetworkStream.Write(buffer, 0, buffer.Length);
            NetworkStream.Flush();
        }

        private string ConvertToFormatedInput(string userInput)
        {
            if (userInput.StartsWith("changename") || userInput.StartsWith("changecolor") || userInput.StartsWith("exit"))
            {
                return "Action " + "|" + this.clientDetails.ToString() + "|" + userInput;
            }

            return "Message " + "|" + this.clientDetails.ToString() + "|" + userInput;
        }

        private void ProcessCommand(object sender, CommandRecievedEventArgs e)
        {
            string[] arr = e.Command.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length != 5)
            {
                return;  // otherwise, not correct format, could be empty message.
            }

            if ("Message ".StartsWith(arr[0]))
            {
                this.DisplayMsg(arr);
            }

            bool setNewProperties = this.clientDetails.Id == arr[1];
            string[] commandArray = arr[4].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.ProcessComand(setNewProperties, commandArray);
        }

        private void DisplayMsg(string[] arr)
        {
            Message message = Message.FromRawMessage(arr);
            string timeNow = GetTimenow();

            Console.ForegroundColor = message.ClientDetails.ConsoleColor;
            ConsoleSize.CheckConsoleSize(120, 30);
            Console.WriteLine(timeNow + message.ClientDetails.Nickname + " says:  " + message.Text);

            Console.ResetColor();
        }

        private void ProcessComand(bool setNewProperties, string[] commandArray)
        {
            if (commandArray[0] == "exit")
            {
                if (setNewProperties)
                {
                    this.StopListen();
                }
                else
                {
                    string timeNow = GetTimenow();
                    Console.WriteLine(timeNow + commandArray[1] + " has left the session.");
                }
            }

            if (commandArray[0] == "init")
            {
                this.clientDetails = ClientDetails.InitWithValues(commandArray[1], commandArray[2], ColorParser.ParseColor(commandArray[3]));
            }

            if (commandArray.Length != 2)
            {
                return;
            }

            if (commandArray[0] == "changename")
            {
                string oldName = this.clientDetails.Nickname;

                if (setNewProperties)
                {
                    this.clientDetails.Nickname = commandArray[1];
                }

                string timeNow = GetTimenow();
                Console.WriteLine(timeNow + oldName + " has changed their name to " + commandArray[1]);
            }

            if (commandArray[0] == "changecolor")
            {
                ConsoleColor color = ColorParser.ParseColor(commandArray[1]);
                if (setNewProperties)
                {
                    this.clientDetails.ConsoleColor = color;
                }

                string timeNow = GetTimenow();
                Console.WriteLine(timeNow + this.clientDetails.Nickname + " has changed their color to " + color);
                Console.ResetColor();
            }
        }

        private TcpClient GetValidServerConnection()
        {
            TcpClient validServerConnection;
            do
            {
                IPAddress ipAddress = this.GetValidIpFromUser();
                int port = this.SelectRemotePort();
                validServerConnection = this.GetValidServerConnection(ipAddress, port);
            }
            while (validServerConnection == null);

            return validServerConnection;
        }

        private IPAddress GetValidIpFromUser()
        {
            IPAddress ipAddress;
            bool parseSuccessful;
            do
            {
                Console.WriteLine("Please enter a valid IP address: ");
                parseSuccessful = IPAddress.TryParse(Console.ReadLine(), out ipAddress);
            }
            while (!parseSuccessful);

            return ipAddress;
        }

        private int SelectRemotePort()
        {
            bool validPortSelected = false;
            int selectedPort;
            do
            {
                Console.WriteLine("Please enter a valid port number:");
                bool isInt = int.TryParse(Console.ReadLine(), out selectedPort);

                if (isInt)
                {
                    validPortSelected = true;
                }
            } 
            while (!validPortSelected);

            return selectedPort;
        }

        private TcpClient GetValidServerConnection(IPAddress ipAddress, int port)
        {
            try
            {
                // skipped using connect method because this does it anyway.
                TcpClient client = new TcpClient(ipAddress.ToString(), port);
                return client;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void StopListen()
        {
            this.clinetRunning = false;
            NetworkStream.Close();
            this.tcpClient.Close();
            this.tcpThreadLinstner.StopListen();
        }
    }
}
