using System;

namespace AsbDemo.Core
{
    public static class Consts
    {
        public const string Endpoint = "sb://kros-gabo-asb.servicebus.windows.net/";
        public const string CstrManagement = "Endpoint=" + Endpoint + ";SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=yLFRi2kLmN/mkduv6UZ3JvDk/51xRZgHlLudeopPoLc=";
        public const string DemoQueueName = "demo-queue";
        public const string DemoTopicName = "demo-topic";
        public const string DemoSubscriptionName = "demo-subscription";

        public const int DefaultMaxDeliveryCount = 10;

        public static readonly TimeSpan DefaultProcessTime = TimeSpan.FromSeconds(1);

        public static readonly TimeSpan DefaultMessageTimeToLive = TimeSpan.FromMinutes(3);
        public static readonly TimeSpan DefaultTokenTimeToLive = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan DefaultLockDuration = TimeSpan.FromSeconds(30);
        public static readonly TimeSpan DefaultAutoDeleteOnIdle = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan DefaultDuplicateDetectionWindow = TimeSpan.FromMinutes(5);
    }
}
