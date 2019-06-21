using AsbDemo.Core;
using System;
using System.Threading.Tasks;

namespace AsbDemo.Topic.Receiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Helper.WriteLine("Press Enter any time to finish." + Environment.NewLine, ConsoleColor.Green);

            var options = Options.Parse(args);

            if (!string.IsNullOrWhiteSpace(options.Postfix))
            {
                options.SubscriptionName += "-" + options.Postfix;
            }

            //if (options.Priority.HasValue && (options.Priority != Priority.Default))
            //{
            //    options.SubscriptionName += "-" + options.Priority.Value.ToString();
            //}

            IReceiver receiver = new AzureSubscriptionReceiver(options);

            Helper.WriteLine($"Receiver type: {receiver.GetType().Name}", ConsoleColor.Yellow);
            await receiver.StartReceivingMessages();

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            await receiver.CloseAsync();
        }
    }
}
