using AsbDemo.Core;
using System;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Helper.WriteLine("Press Enter any time to finish." + Environment.NewLine, ConsoleColor.Green);

            var options = Options.Parse(args);
            options.ConnectionString = Consts.CstrManagement;
            options.QueueName = Consts.DemoQueueName;

            //await SendUsingMassTransit(options);
            await SendUsingAzure(options);
        }

        static async Task SendUsingAzure(Options options)
        {
            var sender = new DemoMessageSender(options);
            await sender.StartSending();
        }

        static async Task SendUsingMassTransit(Options options)
        {
            var sender = new MassTransitMessageSender(options);
            await sender.StartSending();
        }
    }
}
