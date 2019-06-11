using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;

namespace AsbDemo.Core
{
    public static class Helper
    {
        public const string PriorityKey = "Priority";

        private static int _messageCounter = 0;


        public static Message CreateAzureMessage(TimeSpan? timeToLive = null)
        {
            DemoMessage demoMsg = CreateMessage();
            return new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(demoMsg)))
            {
                ContentType = "application/json",
                MessageId = demoMsg.Id,
                TimeToLive = timeToLive ?? TimeSpan.FromMinutes(2)
            };
        }

        public static DemoMessage CreateMessage()
        {
            int currentCounter = Interlocked.Increment(ref _messageCounter);
            return new DemoMessage()
            {
                Id = currentCounter.ToString(),
                Value = $"Lorem ipsum - {currentCounter}"
            };
        }

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

        public static void Write(string message)
        {
            lock (Console.Out)
            {
                Console.Write(message);
            }
        }

        public static void Write(string message, ConsoleColor color)
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = color;
                Console.Write(message);
                Console.ResetColor();
            }
        }
    }
}
