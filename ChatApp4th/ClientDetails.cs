namespace ChatApp4th
{
    using System;

    public class ClientDetails
    {
        private ClientDetails(string id, string nickname, ConsoleColor consoleColor)
        {
            this.ConsoleColor = consoleColor;
            this.Nickname = nickname;
            this.Id = id;
        }

        public string Nickname
        {
            get; set;
        }

        public string Id
        {
            get; private set;
        }

        public ConsoleColor ConsoleColor
        {
            get; set;
        }

        public static ClientDetails InitWithId(string id)
        {
            return new ClientDetails(id, id, ConsoleColor.Gray);
        }

        public static ClientDetails InitWithDefaults()
        {
            return new ClientDetails("0", "0", ConsoleColor.Gray);
        }

        public static ClientDetails InitWithValues(string id, string nickname, ConsoleColor consoleColor)
        {
            return new ClientDetails(id, nickname, consoleColor);
        }

        public override string ToString()
        {
            return this.Id + "|" + this.Nickname + "|" + ConsoleColor;
        }
    }
}
