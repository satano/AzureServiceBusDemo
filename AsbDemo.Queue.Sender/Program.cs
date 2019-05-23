using AsbDemo.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Sender
{
    class Program
    {
        private static MessageSender _sender;

        static async Task Main(string[] args)
        {
            Helper.WriteLine("Press Enter any time to finish." + Environment.NewLine, ConsoleColor.Green);

            var settings = Settings.Parse(args);
            settings.ConnectionString = Settings.CstrDemoSender;

            var tokenSource = new CancellationTokenSource();

            _sender = new MessageSender(settings);
            var senderTask = Task.Run(() =>_sender.SendMessagesAsync(tokenSource.Token));

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            tokenSource.Cancel();

            await senderTask;
        }
    }
}
