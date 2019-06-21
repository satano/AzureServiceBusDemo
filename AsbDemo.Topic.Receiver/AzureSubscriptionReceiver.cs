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
    class AzureSubscriptionReceiver : IReceiver
    {
        private readonly Options _options;
        private SubscriptionClient _client;

        private static async Task CreateSubscription(string topicName, string subscriptionName)
        {
            var management = new ManagementClient(Consts.CstrManagement);
            await management.CreateSubscriptionIfNotExistsAsync(
                topicName,
                subscriptionName,
                s =>
                {
                    s.DefaultMessageTimeToLive = TimeSpan.FromMinutes(3);
                });
        }

        public AzureSubscriptionReceiver(Options options)
        {
            _options = options;
        }

        private async Task<SubscriptionClient> CreateClient()
        {
            RuleDescription rule = null;
            if (_options.Priority.HasValue && (_options.Priority != Priority.Default))
            {
                string priorityStr = _options.Priority.Value.ToString();
                //Filter filter = new SqlFilter($"Priority = '{priorityStr}'");

                var correlationFilter = new CorrelationFilter();
                correlationFilter.Properties["Priority"] = priorityStr;

                rule = new RuleDescription()
                {
                    Name = $"Priority-{priorityStr}",
                    Filter = correlationFilter
                };
            }

            var subscription = new SubscriptionClient(_options.ConnectionString, _options.TopicName, _options.SubscriptionName);
            if (rule != null)
            {
                await subscription.RemoveDefaultRuleAsync();
                await subscription.AddRuleIfNotExistsAsync(rule);
            }
            return subscription;
        }

        public async Task StartReceivingMessages()
        {
            await CreateSubscription(_options.TopicName, _options.SubscriptionName);
            _client = await CreateClient();

            Helper.WriteLine($"Started receiving messages.", ConsoleColor.Magenta);

            var handlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
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
                    priority = Program.ParsePriority((string)priorityStr);
                }
                await Program.ProcessMessage(receivedMessage, _options.ProcessTime, priority);

                if (!token.IsCancellationRequested)
                {
                    await _client.CompleteAsync(message.SystemProperties.LockToken);
                }
            }, handlerOptions);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs e)
        {
            Helper.WriteLine($"Exception: {e.Exception.Message}", ConsoleColor.Red);
            return Task.CompletedTask;
        }

        public async Task CloseAsync() => await _client.CloseAsync();
    }
}
