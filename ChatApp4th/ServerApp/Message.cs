namespace ChatApp4th.ServerApp
{
    public class Message
    {
        private Message(ClientDetails clientDetails, string text)
        {
            this.ClientDetails = clientDetails;
            this.Text = text;
        }

        public string Text
        {
            get; 
            private set;
        }

        public ClientDetails ClientDetails
        {
            get; 
            private set;
        }

        public static Message FromRawMessage(string[] rawInput)
        {
            string id = rawInput[1];
            string nickName = rawInput[2];
            string color = rawInput[3];

            ClientDetails clientDetails = ClientDetails.InitWithValues(id, nickName, ColorParser.ParseColor(color));
            string text = rawInput[4];

            return new Message(clientDetails, text);
        }

        public static Message FromChatClientAndText(ClientDetails clientDetails, string text)
        {
            return new Message(clientDetails, text);
        }
    }
}
