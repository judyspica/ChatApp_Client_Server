namespace ChatApp4th.ClientApp
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class TcpMessagesThreadLinstner
    {
        private readonly TcpClient tcpClient;
        private readonly Thread msgListenerThread;
        private readonly byte[] buffer = new byte[1024];
        private bool isServerRunning = false;

        public TcpMessagesThreadLinstner(TcpClient tcpClinet)
        {
            this.tcpClient = tcpClinet;
            this.isServerRunning = true;

            this.msgListenerThread = new Thread(this.Listen)
            {
                Name = "listener_thread"
            };
            this.msgListenerThread.Start();
        }

        public event EventHandler<CommandRecievedEventArgs> CommandReceived;

        public void StopListen()
        {
            this.isServerRunning = false;
        }

        private void Listen()
        {
            while (this.isServerRunning)
            {
                try
                {
                    int numberOfBytesRead = this.tcpClient.GetStream().Read(this.buffer, 0, this.buffer.Length);
                    string command = Encoding.ASCII.GetString(this.buffer, 0, numberOfBytesRead);

                    if (this.CommandReceived != null)
                    {
                        this.CommandReceived(this, new CommandRecievedEventArgs(command.ToString()));
                    }
                }
                catch (Exception e)
                {
                    this.tcpClient.Close();
                    Console.WriteLine("exception occured you may have forcibly closed the server\n" + e.Message);
                    Console.ReadLine();
                }
            }
        }
    }
}
