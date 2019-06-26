using AsbDemo.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Helper.WriteLine("Press Enter any time to finish." + Environment.NewLine, ConsoleColor.Green);
            var options = Options.Parse(args);

            //ISender sender = new AzureCommandSender(options);
            ISender sender = new MassTransitCommandSender(options);

            Helper.WriteLine($"Sender type: {sender.GetType().Name}", ConsoleColor.Yellow);
            var tokenSource = new CancellationTokenSource();
            var senderTask = Task.Run(() => sender.SendMessagesAsync(tokenSource.Token));

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            tokenSource.Cancel();

            await senderTask;
            await sender.CloseAsync();
        }
    }
}
