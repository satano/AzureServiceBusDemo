using AsbDemo.Core;
using System;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Helper.WriteLine("Press Enter any time to finish." + Environment.NewLine, ConsoleColor.Green);
            var options = Options.Parse(args);

            //IReceiver receiver = new AzureCommandReceiver(options);
            IReceiver receiver = new MassTransitCommandFluentReceiver(options);

            Helper.WriteLine($"Receiver type: {receiver.GetType().Name}", ConsoleColor.Yellow);
            await receiver.StartReceivingMessages();

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);

            await receiver.CloseAsync();
        }

        internal static async Task ProcessMessage(IDemoMessage message, TimeSpan processTime)
        {
            Helper.WriteLine($"Received message: {message.Id} | {message.Value}", ConsoleColor.White);
            await Task.Delay(processTime);
        }
    }
}
