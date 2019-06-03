using AsbDemo.Core;
using Kros.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Topic.Sender
{
    class DemoTopicSender
    {
        public const int DefaultSleepTimeInSeconds = 1;

        private readonly string _senderId = Guid.NewGuid().ToString();
        private int _messageCounter = 0;
        private readonly TopicClient _client;
        private readonly TimeSpan _sleepTime;

        public DemoTopicSender(Options options)
        {
            _client = CreateClient(options.ConnectionString, options.TopicName);
            _sleepTime = options.ProcessTime;
        }

        public static async Task CreateTopic(Options options)
        {
            var management = new ManagementClient(Options.CstrManagement);
            await management.CreateTopicIfNotExistsAsync(
                options.TopicName,
                s =>
                {
                    s.DefaultMessageTimeToLive = TimeSpan.FromMinutes(3);
                });
        }

        private TopicClient CreateClient(string connectionString, string topicName)
        {
            return new TopicClient(connectionString, topicName);
        }

        public async Task SendMessagesAsync(CancellationToken token)
        {
            int i = 0;
            Helper.WriteLine($"Started sending messages. Sender ID: {_senderId}{Environment.NewLine}", ConsoleColor.Magenta);
            while (!token.IsCancellationRequested)
            {
                i++;
                Priority priority = i % 3 == 0 ? Priority.High : Priority.Default;
                priority = i % 5 == 0 ? Priority.Low : priority;
                Message message = CreateMessage(priority);
                await _client.SendAsync(message);

                Helper.WriteLine($"Sender Id: {_senderId}, Message Id: {message.MessageId}, Priority: {priority}", ConsoleColor.Yellow);
                await Task.Delay(_sleepTime);
            }
        }

        private Message CreateMessage(Priority priority)
        {
            int currentCounter = Interlocked.Increment(ref _messageCounter);
            var message = new DemoMessage() { Value = $"Hello world - {currentCounter}" };

            var sbMessage = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)))
            {
                ContentType = "application/json",
                MessageId = currentCounter.ToString(),
                TimeToLive = TimeSpan.FromMinutes(2)
            };
            if (priority != Priority.Default)
            {
                sbMessage.UserProperties[Helper.PriorityKey] = priority.ToString();
            }
            return sbMessage;
        }

        public async Task CloseAsync() => await _client.CloseAsync();
    }
}
