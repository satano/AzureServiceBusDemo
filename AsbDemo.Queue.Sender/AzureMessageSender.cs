using AsbDemo.Core;
using Kros.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Sender
{
    class AzureMessageSender : ISender
    {
        private readonly Options _options;
        private QueueClient _client;

        public AzureMessageSender(Options options)
        {
            _options = options;
        }

        public async Task SendMessagesAsync(CancellationToken token)
        {
            _client = CreateClient();
            await CreateQueue();
            Helper.WriteLine($"Started sending messages.", ConsoleColor.Magenta);
            while (!token.IsCancellationRequested)
            {
                Message message = Helper.CreateAzureMessage();
                await _client.SendAsync(message);

                Helper.WriteLine($"Message sent: Id = {message.MessageId}", ConsoleColor.Yellow);
                await Task.Delay(_options.ProcessTime);
            }
        }

        private QueueClient CreateClient()
            => new QueueClient(_options.ConnectionString, _options.QueueName);

        private async Task CreateQueue()
        {
            var management = new ManagementClient(_options.ConnectionString);
            await management.CreateQueueIfNotExistsAsync(_options.QueueName, queue =>
            {
                queue.DefaultMessageTimeToLive = Consts.DefaultMessageTimeToLive;
            });
        }

        public async Task CloseAsync() => await _client.CloseAsync();
    }
}
