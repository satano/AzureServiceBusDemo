using AsbDemo.Core;
using System;

namespace AsbDemo.Queue.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Helper.WriteLine("Press Enter any time to finish." + Environment.NewLine, ConsoleColor.Green);

            var options = Options.Parse(args);
            options.ConnectionString = Options.CstrDemoReceiver;
            options.QueueName = Options.DemoQueueName;

            var receiver = new MessageReceiver(options);
            receiver.ReceiveMessages();

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
        }
    }
}
