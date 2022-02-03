namespace ChatApp4th.ServerApp
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    public class ChatClient
    {
        private readonly TcpClient tcpClient;
        private readonly ClientDetails clientDetails;
        private readonly byte[] buffer = new byte[1024];
        private bool isListening = false;

        public ChatClient(TcpClient tcpClient, ClientDetails clientDetails)
        {
            this.tcpClient = tcpClient;
            this.isListening = true;
            this.clientDetails = clientDetails;
        }

        public event EventHandler<MessageRecievedEventArgs> MessageReceived;

        private NetworkStream NetworkStream
        {
            get
            {
                return this.tcpClient.GetStream();
            }
        }

        public void SendTextMessage(string text)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(text);
            this.NetworkStream.Write(sendBuffer, 0, sendBuffer.Length);
            this.NetworkStream.Flush();
        }

        public void StopListening()
        {
            this.isListening = false;
        }

        public void StartListening()
        {
            try
            {
                while (this.isListening)
                {
                    int numberOfBytesRead = this.NetworkStream.Read(this.buffer, 0, this.buffer.Length);
                    string msg = Encoding.ASCII.GetString(this.buffer, 0, numberOfBytesRead);
                    ConsoleSize.CheckConsoleSize(120, 30);
                    DateTime now = DateTime.Now;
                    string timeNow = now.ToString("HH:mm:ss ");
                    Console.WriteLine(timeNow + " Server recieved message with the length of " + numberOfBytesRead + " bytes, will process it and send to all clients.");

                    if (this.MessageReceived != null)
                    {
                        this.MessageReceived(this, new MessageRecievedEventArgs(this.clientDetails, msg.ToString()));
                    }
                }
            }
            catch (System.IO.IOException)
            {
                this.StopListening();
            }
            catch (InvalidOperationException)
            {
                this.StopListening();
            }
            catch (Exception)
            {
                // start praying does not get here.
            }
        }

        public void InitActiveClient()
        {
            int numberOfBytesRead = this.NetworkStream.Read(this.buffer, 0, this.buffer.Length);
            string message = Encoding.ASCII.GetString(this.buffer, 0, numberOfBytesRead);
            string[] arr = message.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.clientDetails.Nickname = arr[2];

            // command |taget client|command type and values, space speerated
            string initMessage = "command |" + this.clientDetails.ToString() + "|init " + this.clientDetails.Id + " " + this.clientDetails.Nickname + " " + this.clientDetails.ConsoleColor;
            ConsoleSize.CheckConsoleSize(120, 30);
            this.SendTextMessage(initMessage);
        }

        public ClientDetails GetClientDetails()
        {
            return this.clientDetails;
        }
    }
}
