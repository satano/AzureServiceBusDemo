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

        public TimeSpan ProcessTime { get; set; } = DefaultProcessTime;
        public bool AutoComplete { get; set; } = DefaultAutoComplete;
        public int MaxConcurrentCalls { get; set; } = DefaultMaxConcurrentCalls;

        #endregion

        #region Helpers

        public static string DemoQueueName => "demo-queue";
        public static string CstrDemoSender => "Endpoint=sb://kros-gabo-asb.servicebus.windows.net/;SharedAccessKeyName=SendOnly;SharedAccessKey=1CGiP6AWe7UAXuQktBBODQrcC6C9YdZjHxLg8jvTU7o=";
        public static string CstrDemoReceiver => "Endpoint=sb://kros-gabo-asb.servicebus.windows.net/;SharedAccessKeyName=ListenOnly;SharedAccessKey=/hp3aDZXL3IlPuE+wqayr3YBuR6bgF8oG63H6iJx1nM=";
        public static string CstrDemoTopic => "Endpoint=sb://kros-gabo-asb.servicebus.windows.net/;SharedAccessKeyName=AsbDemo;SharedAccessKey=IYgXqLqNykN+rYJNDCt1miPhcM8h2nf6NxHjCRIp3w4=;EntityPath=demo-topic";

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
            }
            return result;
        }

        #endregion
    }
}
