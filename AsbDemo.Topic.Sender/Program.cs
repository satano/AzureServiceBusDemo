using AsbDemo.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Topic.Sender
{
    class Program
    {
        private static DemoTopicSender _sender;

        static async Task Main(string[] args)
        {
            Helper.WriteLine("Press Enter any time to finish." + Environment.NewLine, ConsoleColor.Green);

            var options = Options.Parse(args);
            options.ConnectionString = Consts.CstrManagement;
            options.TopicName = Consts.DemoTopicName;

            await DemoTopicSender.CreateTopic(options);

            var tokenSource = new CancellationTokenSource();

            _sender = new DemoTopicSender(options);
            var senderTask = Task.Run(() => _sender.SendMessagesAsync(tokenSource.Token));

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            tokenSource.Cancel();

            await senderTask;
            await _sender.CloseAsync();
        }
    }
}
