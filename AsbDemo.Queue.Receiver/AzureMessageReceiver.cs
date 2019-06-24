using AsbDemo.Core;
using Kros.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    class AzureMessageReceiver : IReceiver
    {
        private readonly Options _options;
        private QueueClient _client;

        public AzureMessageReceiver(Options options)
        {
            _options = options;
        }

        public async Task StartReceivingMessages()
        {
            await CreateQueue();
            _client = CreateClient();

            Helper.WriteLine($"Started receiving messages.{Environment.NewLine}", ConsoleColor.Magenta);

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
                await Program.ProcessMessage(receivedMessage, _options.ProcessTime);

                if (!token.IsCancellationRequested)
                {
                    await _client.CompleteAsync(message.SystemProperties.LockToken);
                }
            }, options);
        }

        private QueueClient CreateClient() => new QueueClient(_options.ConnectionString, _options.QueueName);

        private async Task CreateQueue()
        {
            var management = new ManagementClient(_options.ConnectionString);
            await management.CreateQueueIfNotExistsAsync(_options.QueueName, queue =>
            {
                queue.DefaultMessageTimeToLive = Consts.DefaultMessageTimeToLive;
            });
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs e)
        {
            Helper.WriteLine($"Exception: {e.Exception.Message}", ConsoleColor.Red);
            return Task.CompletedTask;
        }

        public async Task CloseAsync() => await _client.CloseAsync();
    }
}
