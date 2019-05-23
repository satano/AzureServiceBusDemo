using AsbDemo.Core;
using System;

namespace AsbDemo.Queue.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Helper.WriteLine("Press Enter any time to finish." + Environment.NewLine, ConsoleColor.Green);

            var settings = Settings.Parse(args);
            settings.ConnectionString = Settings.CstrDemoReceiver;

            var receiver = new MessageReceiver(settings);
            receiver.ReceiveMessages();

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
        }
    }
}
