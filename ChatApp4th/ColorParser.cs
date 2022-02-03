namespace ChatApp4th
{
    using System;

    public class ColorParser
    {
        public static ConsoleColor ParseColor(string color)
        {
            switch (color.ToLower())
            {
                case "red":
                    return ConsoleColor.Red;
                case "blue":
                    return ConsoleColor.Blue;
                case "yellow":
                    return ConsoleColor.Yellow;
                case "white":
                    return ConsoleColor.White;
                case "magenta":
                    return ConsoleColor.Magenta;
                case "cyan":
                    return ConsoleColor.Cyan;
                default:
                    return ConsoleColor.Gray;
            }
        }
    }
}
