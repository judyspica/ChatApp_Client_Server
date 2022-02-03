namespace ChatApp4th.ServerApp
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class ServerListener
    {
        private readonly Thread listenerThread;
        private readonly TcpListener tcpListener;
        private int currentlyAssignedIds = 0;
        private bool isServerRunning;

        public ServerListener(int port)
        {
            this.tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));

            this.listenerThread = new Thread(this.Listen)
            {
                Name = "listener_thread"
            };
        }

        public event EventHandler<ChatClientConnectedEventArgs> ClientConnected;

        public void Start()
        {
            this.isServerRunning = true;
            this.tcpListener.Start();
            this.listenerThread.Start();
        }

        public void Stop()
        {
            this.isServerRunning = false;
        }

        private void Listen()
        {
            while (this.isServerRunning)
            {
                TcpClient tcpClient = this.tcpListener.AcceptTcpClient();

                // Update how many clients have connected so far.
                this.currentlyAssignedIds++;
                ClientDetails clientDetails = ClientDetails.InitWithId(this.currentlyAssignedIds.ToString());

                // we fire the event in async way because th execution is exiting the loop when firing the event.
                new Thread(() => this.FireClientConnected(tcpClient, clientDetails)).Start();
            }
        }

        private void FireClientConnected(TcpClient tcpClient, ClientDetails clientDetails)
        {
            if (this.ClientConnected != null)
            {
                this.ClientConnected(this, new ChatClientConnectedEventArgs(tcpClient, clientDetails));
            }
        }
    }
}
