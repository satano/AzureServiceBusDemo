﻿using System;

namespace Kros.MassTransit.AzureServiceBus
{
    /// <summary>
    /// Default configuration for Azure via Mass Transit.
    /// </summary>
    internal static class ConfigDefaults
    {
        /// <summary>
        /// Default message time to live in the queue.
        /// </summary>
        public static readonly TimeSpan MessageTimeToLive = TimeSpan.FromMinutes(3);

        /// <summary>
        /// Default setting for move messages to the dead letter queue on expiration (time to live exceeded).
        /// </summary>
        public const bool EnableDeadLetteringOnMessageExpiration = true;

        /// <summary>
        /// Default lock duration for messages read from the queue.
        /// </summary>
        public static readonly TimeSpan LockDuration = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Default value for if the queue should be deleted if idle.
        /// </summary>
        public static readonly TimeSpan AutoDeleteOnIdle = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Default maximum delivery count. A message is automatically deadlettered after this number of deliveries.
        /// </summary>
        public const int MaxDeliveryCount = 10;

        /// <summary>
        /// Default time window for duplicate history.
        /// </summary>
        public static readonly TimeSpan DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
    }
}
