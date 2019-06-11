using AsbDemo.Core;
using Kros.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Sender
{
    class DemoMessageSender
    {
        private readonly Options _options;

        public DemoMessageSender(Options options)
        {
            _options = options;
        }

        public async Task StartSending()
        {
            var management = new ManagementClient(_options.ConnectionString);
            await management.CreateQueueIfNotExistsAsync(_options.QueueName);

            var client = new QueueClient(_options.ConnectionString, _options.QueueName);
            var tokenSource = new CancellationTokenSource();
            var senderTask = Task.Run(() => SendMessagesAsync(client, tokenSource.Token));

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            tokenSource.Cancel();

            await senderTask;
            await client.CloseAsync();
        }

        private async Task SendMessagesAsync(QueueClient client, CancellationToken token)
        {
            Helper.WriteLine($"Started sending messages.{Environment.NewLine}", ConsoleColor.Magenta);
            while (!token.IsCancellationRequested)
            {
                Message message = Helper.CreateAzureMessage();
                await client.SendAsync(message);

                Helper.WriteLine($"Message sent: Id = {message.MessageId}", ConsoleColor.Yellow);
                await Task.Delay(_options.ProcessTime);
            }
        }
    }
}
