using AsbDemo.Core;
using System;
using System.Threading.Tasks;

namespace AsbDemo.Topic.Receiver
{
    class Program
    {
        private static DemoSubscriptionReceiver _receiver;

        static async Task Main(string[] args)
        {
            Helper.WriteLine("Press Enter any time to finish." + Environment.NewLine, ConsoleColor.Green);

            var options = Options.Parse(args);
            options.ConnectionString = Options.CstrDemoReceiver;
            options.TopicName = Options.DemoTopicName;
            options.SubscriptionName = Options.DemoSubscriptionName;

            if (!string.IsNullOrWhiteSpace(options.Postfix))
            {
                options.SubscriptionName += "-" + options.Postfix;
            }

            if (options.Priority.HasValue && (options.Priority != Priority.Default))
            {
                options.SubscriptionName += "-" + options.Priority.Value.ToString();
            }
            await DemoSubscriptionReceiver.CreateSubscription(options);
            _receiver = new DemoSubscriptionReceiver(options);
            _receiver.ReceiveMessages();

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            await _receiver.CloseAsync();
        }
    }
}
