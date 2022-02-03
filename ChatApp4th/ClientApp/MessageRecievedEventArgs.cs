namespace ChatApp4th.ClientApp
{
    using System;

    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageRecievedEventArgs(string msg)
        {
            this.Msg = msg;
        }

        public string Msg
        {
            get;
            private set;
        }
    }
}
