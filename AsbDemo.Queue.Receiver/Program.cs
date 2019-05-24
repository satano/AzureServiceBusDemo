using AsbDemo.Core;
using System;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    class Program
    {
        private static MessageReceiver _receiver;

        static async Task Main(string[] args)
        {
            Helper.WriteLine("Press Enter any time to finish." + Environment.NewLine, ConsoleColor.Green);

            var options = Options.Parse(args);
            options.ConnectionString = Options.CstrDemoReceiver;
            options.QueueName = Options.DemoQueueName;

            _receiver = new MessageReceiver(options);
            _receiver.ReceiveMessages();

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            await _receiver.CloseAsync();
        }
    }
}
