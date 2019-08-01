using AsbDemo.Core;
using Kros.MassTransit.AzureServiceBus;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace AsbDemo.Topic.Receiver
{
    class MassTransitEventFluentReceiver : IReceiver
    {
        private readonly Options _options;
        IBusControl _bus;

        public MassTransitEventFluentReceiver(Options options)
        {
            _options = options;
        }

        public async Task StartReceivingMessages()
        {
            Helper.Write("Starting service bus ...", ConsoleColor.Magenta);

            _bus = new MassTransitForAzureBuilder(_options.ConnectionString)
                .ConfigureServiceBusFactory((busCfg, host) => { })
                .ConfigureSubscription<IDemoMessage>(_options.SubscriptionName,
                    endpointCfg => endpointCfg.Rule = Program.CreateSubscriptionRule(_options.Priority))
                    .AddConsumer<IDemoMessage>(async context =>
                    {
                        Priority? priority = Program.ParsePriority(context.Headers.Get<string>(Helper.PriorityKey, null));
                        await Program.ProcessMessage(context.Message, _options.ProcessTime, priority);
                    })
                .Build();

            Helper.WriteLine($"Started receiving messages from subscription  \"{_options.SubscriptionName}\".",
                ConsoleColor.Magenta);

            await _bus.StartAsync();

            Helper.WriteLine(" started.", ConsoleColor.Magenta);
        }

        public async Task CloseAsync() => await _bus?.StopAsync();
    }
}
