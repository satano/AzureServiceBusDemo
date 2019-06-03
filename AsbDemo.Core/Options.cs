using System;
using System.Text.RegularExpressions;

namespace AsbDemo.Core
{
    public class Options
    {
        #region Options

        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }

        public TimeSpan ProcessTime { get; set; } = DefaultProcessTime;
        public bool AutoComplete { get; set; } = DefaultAutoComplete;
        public int MaxConcurrentCalls { get; set; } = DefaultMaxConcurrentCalls;
        public Priority? Priority { get; set; } = null;
        public string Postfix { get; set; } = null;

        #endregion

        #region Helpers

        public const string DemoQueueName = "demo-queue";
        public const string CstrDemoSender = "Endpoint=sb://kros-gabo-asb.servicebus.windows.net/;SharedAccessKeyName=SendOnly;SharedAccessKey=1CGiP6AWe7UAXuQktBBODQrcC6C9YdZjHxLg8jvTU7o=";
        public const string CstrDemoReceiver = "Endpoint=sb://kros-gabo-asb.servicebus.windows.net/;SharedAccessKeyName=ListenOnly;SharedAccessKey=/hp3aDZXL3IlPuE+wqayr3YBuR6bgF8oG63H6iJx1nM=";

        public const string DemoTopicName = "demo-topic";
        public const string DemoSubscriptionName = "demo-subscription";
        public const string CstrManagement = "Endpoint=sb://kros-gabo-asb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=yLFRi2kLmN/mkduv6UZ3JvDk/51xRZgHlLudeopPoLc=";

        public static TimeSpan DefaultProcessTime => TimeSpan.FromSeconds(1);
        public static bool DefaultAutoComplete => false;
        public static int DefaultMaxConcurrentCalls => 1;

        private static readonly Regex _regex = new Regex("-(?<name>[a-z0-9]+) (?<value>[a-z0-9]+)", RegexOptions.IgnoreCase);

        public static Options Parse(string[] args)
            => (args != null) && (args.Length > 0) ? Parse(string.Join(" ", args)) : new Options();

        public static Options Parse(string args)
        {
            var result = new Options();

            foreach (Match match in _regex.Matches(args))
            {
                string argName = match.Groups["name"].Value;
                string argValue = match.Groups["value"].Value;

                if (nameof(ProcessTime).Equals(argName, StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(argValue, out int processTime))
                    {
                        if (processTime > 0)
                        {
                            result.ProcessTime = TimeSpan.FromMilliseconds(processTime);
                        }
                    }
                }
                else if (nameof(AutoComplete).Equals(argName, StringComparison.OrdinalIgnoreCase))
                {
                    if (bool.TryParse(argValue, out bool autoComplete))
                    {
                        result.AutoComplete = autoComplete;
                    }
                }
                else if (nameof(MaxConcurrentCalls).Equals(argName, StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(argValue, out int maxConcurrentCalls))
                    {
                        if (maxConcurrentCalls > 0)
                        {
                            result.MaxConcurrentCalls = maxConcurrentCalls;
                        }
                    }
                }
                else if (nameof(Priority).Equals(argName, StringComparison.OrdinalIgnoreCase))
                {
                    if (Enum.TryParse(argValue, ignoreCase: true, out Priority priority))
                    {
                        result.Priority = priority;
                    }
                }
                else if (nameof(Postfix).Equals(argName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Postfix = string.IsNullOrWhiteSpace(argValue) ? null : argValue.Trim();
                }
            }
            return result;
        }

        #endregion
    }
}
