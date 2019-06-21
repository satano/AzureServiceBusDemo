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

            //IReceiver receiver = await ReceiveUsingAzure(options);
            IReceiver receiver = await ReceiveUsingMassTransit(options);

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);

            await receiver.CloseAsync();
        }

        static async Task<DemoMessageReceiver> ReceiveUsingAzure(Options options)
        {
            var receiver = new DemoMessageReceiver(options);
            receiver.StartReceivingMessages();
            return await Task.FromResult(receiver);
        }

        static async Task<MassTransitMessageReceiver> ReceiveUsingMassTransit(Options options)
        {
            var receiver = new MassTransitMessageReceiver(options);
            await receiver.StartReceivingMessages();
            return receiver;
        }
    }
}
