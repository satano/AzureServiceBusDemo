using AsbDemo.Core;
using Microsoft.Azure.ServiceBus;
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
            UpdateSubscriptionName(options);

            //IReceiver receiver = new AzureEventReceiver(options);
            IReceiver receiver = new MassTransitEventReceiver(options);

            Helper.WriteLine($"Receiver type: {receiver.GetType().Name}", ConsoleColor.Yellow);
            await receiver.StartReceivingMessages();

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            await receiver.CloseAsync();
        }

        internal static async Task ProcessMessage(IDemoMessage message, TimeSpan processTime, Priority? priority)
        {
            string priorityStr = priority.HasValue ? priority.ToString() : string.Empty;
            Helper.WriteLine($"Received message: {message.Value}, priority = {priorityStr}", ConsoleColor.White);
            await Task.Delay(processTime);
        }

        internal static Priority? ParsePriority(string priorityStr)
        {
            Priority? priority = null;
            if (!string.IsNullOrEmpty(priorityStr) && Enum.TryParse(priorityStr, out Priority msgPriority))
            {
                priority = msgPriority;
            }
            return priority;
        }

        static void UpdateSubscriptionName(Options options)
        {
            if (!string.IsNullOrWhiteSpace(options.Postfix))
            {
                options.SubscriptionName += "-" + options.Postfix;
            }
            if (options.Priority.HasValue && (options.Priority != Priority.Default))
            {
                options.SubscriptionName += "-" + options.Priority.Value.ToString();
            }
        }

        internal static RuleDescription CreateSubscriptionRule(Priority? priority)
        {
            RuleDescription rule = null;
            if (priority.HasValue && (priority != Priority.Default))
            {
                string priorityStr = priority.Value.ToString();
                //var filter = new SqlFilter($"Priority = '{priorityStr}'");

                var correlationFilter = new CorrelationFilter();
                correlationFilter.Properties["Priority"] = priorityStr;

                rule = new RuleDescription()
                {
                    Name = $"Priority-{priorityStr}",
                    Filter = correlationFilter
                };
            }
            return rule;
        }
    }
}
