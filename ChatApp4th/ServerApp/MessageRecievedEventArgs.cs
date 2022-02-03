namespace ChatApp4th.ServerApp
{
    using System;

    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageRecievedEventArgs(ClientDetails client, string msg)
        {
            this.Client = client;
            this.Msg = msg;
        }

        public ClientDetails Client
        {
            get;
            private set;
        }

        public string Msg
        {
            get;
            private set;
        }
    }
}
