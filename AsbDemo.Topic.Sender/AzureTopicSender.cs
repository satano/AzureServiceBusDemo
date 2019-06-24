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
    class AzureTopicSender : ISender
    {
        public const int DefaultSleepTimeInSeconds = 1;

        private static int _priorityCounter = 0;
        public static Priority GetPriority()
        {
            _priorityCounter++;
            Priority priority = _priorityCounter % 3 == 0 ? Priority.High : Priority.Default;
            return _priorityCounter % 5 == 0 ? Priority.Low : priority;
        }

        private readonly Options _options;
        private TopicClient _client;

        public AzureTopicSender(Options options)
        {
            _options = options;
        }

        private async Task CreateTopic()
        {
            var management = new ManagementClient(Consts.CstrManagement);
            await management.CreateTopicIfNotExistsAsync(
                _options.TopicName,
                s =>
                {
                    s.DefaultMessageTimeToLive = Consts.DefaultMessageTimeToLive;
                });
        }

        private TopicClient CreateClient(string connectionString, string topicName)
            => new TopicClient(connectionString, topicName);

        public async Task SendMessagesAsync(CancellationToken token)
        {
            _client = CreateClient(_options.ConnectionString, _options.TopicName);
            await CreateTopic();
            Helper.WriteLine("Started sending messages.", ConsoleColor.Magenta);
            while (!token.IsCancellationRequested)
            {
                Priority priority = GetPriority();
                Message message = CreateMessage(priority);
                await _client.SendAsync(message);

                Helper.WriteLine($"Message sent: Id = {message.MessageId}, Priority = {priority}", ConsoleColor.Yellow);
                await Task.Delay(_options.ProcessTime);
            }
        }

        private Message CreateMessage(Priority priority)
        {
            Message message = Helper.CreateAzureMessage();
            if (priority != Priority.Default)
            {
                message.UserProperties[Helper.PriorityKey] = priority.ToString();
            }
            return message;
        }

        public async Task CloseAsync() => await _client?.CloseAsync();
    }
}
