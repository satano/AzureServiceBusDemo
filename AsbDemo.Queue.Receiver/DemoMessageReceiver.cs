using AsbDemo.Core;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    class DemoMessageReceiver
    {
        public const int DefaultSleepTimeInSeconds = 1;

        private readonly string _receiverId = Guid.NewGuid().ToString();
        private readonly QueueClient _client;
        private readonly TimeSpan _processTime;
        private readonly Options _options;

        public DemoMessageReceiver(Options options)
        {
            _options = options;
            _client = CreateClient(options.ConnectionString, options.QueueName);
            _processTime = options.ProcessTime;
        }

        private QueueClient CreateClient(string connectionString, string queueName)
        {
            return new QueueClient(connectionString, queueName);
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
                await ProcessMessage(receivedMessage);

                if (!token.IsCancellationRequested)
                {
                    await _client.CompleteAsync(message.SystemProperties.LockToken);
                }
            }, options);
        }

        private async Task ProcessMessage(DemoMessage message)
        {
            Helper.WriteLine($"{_receiverId} Received message: {message.Value}", ConsoleColor.White);
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
