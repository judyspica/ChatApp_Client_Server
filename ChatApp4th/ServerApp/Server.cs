namespace ChatApp4th.ServerApp
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class Server
    {
        private readonly List<ChatClient> clients;
        private readonly List<Message> messages;
        private ServerListener server;
        private Thread keyListenerThread;

        public Server()
        {
            this.clients = new List<ChatClient>();
            this.messages = new List<Message>();
        }

        public void IntiServer()
        {
            int port = this.SelectLocalPort();
            this.server = new ServerListener(port);
            this.server.ClientConnected += this.ClientConnected;
            this.server.Start();

            this.keyListenerThread = new Thread(this.WatchKeyboard)
            {
                Name = "KeyboardListenerThread"
            };
            this.keyListenerThread.Start();
        }

        public void WatchKeyboard()
        {
            ConsoleKey input;
            do
            {
                input = Console.ReadKey(true).Key;
            }
            while (input != ConsoleKey.Escape);
            this.Stop();
        }

        public void Stop()
        {
            this.server.Stop();
            foreach (ChatClient client in this.clients)
            {
                client.StopListening();
            }
        }

        private static string GetTimenow()
        {
            DateTime now = DateTime.Now;
            string timeNow = now.ToString("HH:mm:ss ");
            return timeNow;
        }

        private void ClientConnected(object sender, ChatClientConnectedEventArgs e)
        {
            ChatClient chatClient = new ChatClient(e.TcpClient, e.ClientDetails);
            chatClient.InitActiveClient();
            chatClient.MessageReceived += this.MessageRecieved;

            string message = "Message " + "|" + chatClient.GetClientDetails().ToString() + "|" + GetTimenow() + ": New client just connected With the nickname: " + chatClient.GetClientDetails().Nickname;
            Console.WriteLine(message);

            foreach (ChatClient cc in this.clients)
            {
                cc.SendTextMessage(message);
            }

            this.clients.Add(chatClient);
            chatClient.StartListening();
        }

        private void MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            foreach (ChatClient chatClient in this.clients)
            {
                this.ProcessCommand(e.Client, e.Msg);
                chatClient.SendTextMessage(e.Msg);
            }

            Console.WriteLine(e.Msg);
        }

        private void ProcessCommand(ClientDetails client, string msg)
        {
            string[] arr = msg.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 5)
            {
                return;  // otherwise, malformed msg (like sending empty msg)
            }

            if (arr[0] == "Message ")
            {
                Message message = Message.FromRawMessage(arr);
                this.messages.Add(message);
            }
            else
            {
                this.ChangeChatClinetDetails(client, arr);
            }
        }

        private void ChangeChatClinetDetails(ClientDetails client, string[] arr)
        {
            if (arr[4] == "exit")
            {
                List<ChatClient> clientToRemove = new List<ChatClient>();
                foreach (ChatClient cc in this.clients)
                {
                    if (cc.GetClientDetails().Id == client.Id)
                    {
                        clientToRemove.Add(cc);
                    }
                }

                foreach (ChatClient cc in clientToRemove)
                {
                    cc.StopListening();
                    this.clients.Remove(cc);
                }

                string timeNow = GetTimenow();
                Console.WriteLine(timeNow + " " + client.Id + " " + "Client has gotten out ");
            }

            string[] commandArr = arr[4].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (commandArr.Length != 2)
            {
                return;
            }

            if (commandArr[0] == "changename")
            {
                client.Nickname = commandArr[1];

                string timeNow = GetTimenow();
                Console.WriteLine(timeNow + " " + client.Id + " name had been changed ");
            }

            if (commandArr[0] == "changecolor")
            {
                client.ConsoleColor = ColorParser.ParseColor(commandArr[1]);

                string timeNow = GetTimenow();
                Console.WriteLine(timeNow + " " + client.Id + " console color had been changed to " + client.ConsoleColor);
            }
        }

        private int SelectLocalPort()
        {
            bool validPortSelected = false;
            int selectedPort;
            do
            {
                Console.WriteLine("Please enter a valid port number:");

                bool isInt = int.TryParse(Console.ReadLine(), out selectedPort);

                if (isInt && this.IsUnusedPort(selectedPort))
                {
                    validPortSelected = true;
                }
            } 
            while (!validPortSelected);

            return selectedPort;
        }

        private bool IsUnusedPort(int selectedPort)
        {
            try
            {
                TcpListener tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, selectedPort));
                tcpListener.Start();
                tcpListener.Stop();

                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Port already in use!");
                return false;
            }
        }
    }
}
