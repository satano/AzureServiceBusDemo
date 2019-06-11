using AsbDemo.Core;
using MassTransit.Transports;
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
            options.ConnectionString = Consts.CstrManagement;
            options.QueueName = Consts.DemoQueueName;

            IReceiver receiver = ReceiveUsingAzure(options);

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);

            await receiver.CloseAsync();
        }

        static DemoMessageReceiver ReceiveUsingAzure(Options options)
        {
            var receiver = new DemoMessageReceiver(options);
            receiver.StartReceivingMessages();
            return receiver;
        }
    }
}
