using AsbDemo.Core;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Sender
{
    class MessageSender
    {
        public const int DefaultSleepTimeInSeconds = 1;

        private readonly string _senderId = Guid.NewGuid().ToString();
        private int _messageCounter = 0;
        private readonly QueueClient _client;
        private readonly TimeSpan _sleepTime;

        public MessageSender(Options options)
        {
            _client = CreateClient(options.ConnectionString, options.QueueName);
            _sleepTime = options.ProcessTime;
        }

        private QueueClient CreateClient(string connectionString, string queueName)
        {
            return new QueueClient(connectionString, queueName);
        }

        public async Task SendMessagesAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Message message = CreateMessage();
                await _client.SendAsync(message);

                Helper.WriteLine($"Sender Id: {_senderId}, Message Id: {message.MessageId}", ConsoleColor.Yellow);
                await Task.Delay(_sleepTime);
            }
        }

        private Message CreateMessage()
        {
            int currentCounter = Interlocked.Increment(ref _messageCounter);
            var message = new DemoMessage() { Value = $"Lorem ipsum {_senderId} - {currentCounter}" };

            return new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)))
            {
                ContentType = "application/json",
                MessageId = currentCounter.ToString(),
                TimeToLive = TimeSpan.FromMinutes(2)
            };
        }
    }
}
