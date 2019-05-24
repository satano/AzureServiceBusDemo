using AsbDemo.Core;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    class MessageReceiver
    {
        public const int DefaultSleepTimeInSeconds = 1;

        private readonly string _receiverId = Guid.NewGuid().ToString();
        private readonly QueueClient _client;
        private readonly TimeSpan _processTime;
        private readonly Options _options;

        public MessageReceiver(Options options)
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
            var options = new MessageHandlerOptions(e => ExceptionReceivedHandler(e))
            {
                AutoComplete = _options.AutoComplete,
                MaxConcurrentCalls = _options.MaxConcurrentCalls
            };

            _client.RegisterMessageHandler(async (message, token) =>
            {

                DemoMessage receivedMessage = JsonConvert.DeserializeObject<DemoMessage>(Encoding.UTF8.GetString(message.Body));

                Helper.WriteLine($"{_receiverId} Received message: {receivedMessage.Value}", ConsoleColor.White);

                await Task.Delay(_processTime);

                await _client.CompleteAsync(message.SystemProperties.LockToken);

            }, options);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs e)
        {
            Helper.WriteLine($"Exception: {e.Exception.Message}", ConsoleColor.Red);
            return Task.CompletedTask;
        }
    }
}
