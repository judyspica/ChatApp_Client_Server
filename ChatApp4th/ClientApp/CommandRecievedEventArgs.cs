namespace ChatApp4th.ClientApp
{
    public class CommandRecievedEventArgs
    {
        public CommandRecievedEventArgs(string command)
        {
            this.Command = command;
        }

        public string Command
        {
            get;
            private set;
        }
    }
}
