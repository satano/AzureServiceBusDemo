using AsbDemo.Core;
using Kros.Azure.ServiceBus;
using Kros.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AsbDemo.Topic.Receiver
{
    class DemoSubscriptionReceiver
    {
        public const int DefaultSleepTimeInSeconds = 1;

        private readonly string _receiverId = Guid.NewGuid().ToString();
        private readonly SubscriptionClient _client;
        private readonly TimeSpan _processTime;
        private readonly Options _options;

        public static async Task CreateSubscription(Options options)
        {
            var management = new ManagementClient(Consts.CstrManagement);
            await management.CreateSubscriptionIfNotExistsAsync(
                options.TopicName,
                options.SubscriptionName,
                s =>
                {
                    s.DefaultMessageTimeToLive = TimeSpan.FromMinutes(3);
                });
        }

        public DemoSubscriptionReceiver(Options options)
        {
            _options = options;
            _client = CreateClient(options).GetAwaiter().GetResult();
            _processTime = options.ProcessTime;
        }

        private async Task<SubscriptionClient> CreateClient(Options options)
        {
            RuleDescription rule = null;
            if (options.Priority.HasValue && (options.Priority != Priority.Default))
            {
                string priorityStr = options.Priority.Value.ToString();
                Filter filter = new SqlFilter($"Priority = '{priorityStr}'");

                var correlationFilter = new CorrelationFilter();
                correlationFilter.Properties["Priority"] = priorityStr;
                filter = correlationFilter;

                rule = new RuleDescription()
                {
                    Name = $"Priority-{priorityStr}",
                    Filter = filter
                };
            }

            var subscription = new SubscriptionClient(options.ConnectionString, options.TopicName, options.SubscriptionName);
            if (rule != null)
            {
                await subscription.RemoveDefaultRuleAsync();
                await subscription.AddRuleIfNotExistsAsync(rule);
            }
            return subscription;
        }

        public void ReceiveMessages()
        {
            Helper.WriteLine($"Started receiving messages. Receiver ID: {_receiverId}{Environment.NewLine}", ConsoleColor.Magenta);
            var options = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = _options.AutoComplete,
                MaxConcurrentCalls = _options.MaxConcurrentCalls
            };

            _client.RegisterMessageHandler(async (message, token) =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                DemoMessage receivedMessage = JsonConvert.DeserializeObject<DemoMessage>(Encoding.UTF8.GetString(message.Body));
                Priority? priority = null;
                if (message.UserProperties.TryGetValue(Helper.PriorityKey, out object priorityStr))
                {
                    if (Enum.TryParse((string)priorityStr, out Priority msgPriority))
                    {
                        priority = msgPriority;
                    }
                }
                await ProcessMessage(receivedMessage, priority);

                if (!token.IsCancellationRequested)
                {
                    await _client.CompleteAsync(message.SystemProperties.LockToken);
                }
            }, options);
        }

        private async Task ProcessMessage(DemoMessage message, Priority? priority)
        {
            string priorityStr = priority.HasValue ? priority.ToString() : string.Empty;
            Helper.WriteLine($"Received message: {message.Value}, priority = {priorityStr}", ConsoleColor.White);
            await Task.Delay(_processTime);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs e)
        {
            Helper.WriteLine($"Exception: {e.Exception.Message}", ConsoleColor.Red);
            return Task.CompletedTask;
        }

        public async Task CloseAsync() => await _client.CloseAsync();
    }
}
