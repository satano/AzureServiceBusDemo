using AsbDemo.Core;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    class DemoMessageReceiver : IReceiver
    {
        public const int DefaultSleepTimeInSeconds = 1;

        private readonly QueueClient _client;
        private readonly Options _options;

        public DemoMessageReceiver(Options options)
        {
            _options = options;
            _client = new QueueClient(options.ConnectionString, options.QueueName);
        }

        public void StartReceivingMessages()
        {
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
                await ProcessMessage(receivedMessage);

                if (!token.IsCancellationRequested)
                {
                    await _client.CompleteAsync(message.SystemProperties.LockToken);
                }
            }, options);
        }

        private async Task ProcessMessage(DemoMessage message)
        {
            Helper.WriteLine($"Received message: {message.Id} | {message.Value}", ConsoleColor.White);
            await Task.Delay(_options.ProcessTime);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs e)
        {
            Helper.WriteLine($"Exception: {e.Exception.Message}", ConsoleColor.Red);
            return Task.CompletedTask;
        }

        public async Task CloseAsync() => await _client.CloseAsync();
    }
}
