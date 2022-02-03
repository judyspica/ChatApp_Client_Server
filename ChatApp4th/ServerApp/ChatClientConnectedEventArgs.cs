namespace ChatApp4th.ServerApp
{
    using System;
    using System.Net.Sockets;

    public class ChatClientConnectedEventArgs : EventArgs
    {
        public ChatClientConnectedEventArgs(TcpClient tcpClient, ClientDetails clientDetails)
        {
            this.TcpClient = tcpClient;
            this.ClientDetails = clientDetails;
        }

        public ClientDetails ClientDetails
        {
            get;
            private set;
        }

        public TcpClient TcpClient
        {
            get;
            private set;
        }
    }
}
