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

            var options = Options.Parse(args);
            options.ConnectionString = Options.CstrDemoSender;
            options.QueueName = Options.DemoQueueName;

            var tokenSource = new CancellationTokenSource();

            _sender = new MessageSender(options);
            var senderTask = Task.Run(() =>_sender.SendMessagesAsync(tokenSource.Token));

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            tokenSource.Cancel();

            await senderTask;
        }
    }
}
