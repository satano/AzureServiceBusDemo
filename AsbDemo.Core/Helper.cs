using System;

namespace AsbDemo.Core
{
    public static class Helper
    {
        public const string PriorityKey = "Priority";

        public static void WriteLine(string message)
        {
            lock (Console.Out)
            {
                Console.WriteLine(message);
            }
        }

        public static void WriteLine(string message, ConsoleColor color)
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}
