using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Threading.Tasks;

namespace Kros.Azure.ServiceBus.Management
{
    public static class ManagementClientExtensions
    {
        public async static Task CreateQueueIfNotExistsAsync(
            this ManagementClient client,
            string queueName,
            Action<QueueDescription> queueSetup)
        {
            if (!await client.QueueExistsAsync(queueName))
            {
                var queue = new QueueDescription(queueName);
                queueSetup?.Invoke(queue);
                await client.CreateQueueAsync(queue);
            }
        }

        public async static Task CreateQueueIfNotExistsAsync(this ManagementClient client, string queueName)
            => await client.CreateQueueIfNotExistsAsync(queueName, null);

        public async static Task CreateQueueIfNotExistsAsync(this ManagementClient client, QueueDescription queue)
        {
            if (!await client.QueueExistsAsync(queue.Path))
            {
                await client.CreateQueueAsync(queue);
            }
        }

        public async static Task CreateTopicIfNotExistsAsync(
            this ManagementClient client,
            string topicName,
            Action<TopicDescription> topicSetup)
        {
            if (!await client.TopicExistsAsync(topicName))
            {
                var topic = new TopicDescription(topicName);
                topicSetup?.Invoke(topic);
                await client.CreateTopicAsync(topic);
            }
        }

        public async static Task CreateTopicIfNotExistsAsync(this ManagementClient client, string topicName)
            => await client.CreateTopicIfNotExistsAsync(topicName, null);

        public async static Task CreateTopicIfNotExistsAsync(this ManagementClient client, TopicDescription topic)
        {
            if (!await client.TopicExistsAsync(topic.Path))
            {
                await client.CreateTopicAsync(topic);
            }
        }

        public async static Task CreateSubscriptionIfNotExistsAsync(
            this ManagementClient client,
            string topicName,
            string subscriptionName,
            Action<SubscriptionDescription> subscriptionSetup)
        {
            if (!await client.SubscriptionExistsAsync(topicName, subscriptionName))
            {
                var subscription = new SubscriptionDescription(topicName, subscriptionName);
                subscriptionSetup?.Invoke(subscription);
                await client.CreateSubscriptionAsync(subscription);
            }
        }

        public async static Task CreateSubscriptionIfNotExistsAsync(
            this ManagementClient client,
            string topicName,
            string subscriptionName)
            => await client.CreateSubscriptionIfNotExistsAsync(topicName, subscriptionName, null);

        public async static Task CreateSubscriptionIfNotExistsAsync(
            this ManagementClient client,
            SubscriptionDescription subscription)
        {
            if (!await client.SubscriptionExistsAsync(subscription.TopicPath, subscription.SubscriptionName))
            {
                await client.CreateSubscriptionAsync(subscription);
            }
        }
    }
}
