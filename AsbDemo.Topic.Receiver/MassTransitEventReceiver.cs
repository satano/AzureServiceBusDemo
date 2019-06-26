using AsbDemo.Core;
using MassTransit;
using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace AsbDemo.Topic.Receiver
{
    class MassTransitEventReceiver : IReceiver
    {
        private readonly Options _options;
        IBusControl _bus;

        public MassTransitEventReceiver(Options options)
        {
            _options = options;
        }

        public async Task StartReceivingMessages()
        {
            _bus = await Helper.StartBusControl((busCfg, host) =>
            {
                busCfg.SubscriptionEndpoint<IDemoMessage>(host, _options.SubscriptionName, endpointCfg =>
                {
                    endpointCfg.Rule = Program.CreateSubscriptionRule(_options.Priority);
                    endpointCfg.Handler<IDemoMessage>(async context =>
                    {
                        Priority? priority = Program.ParsePriority(context.Headers.Get<string>(Helper.PriorityKey, null));
                        await Program.ProcessMessage(context.Message, _options.ProcessTime, priority);
                    });
                });
            });
        }

        public async Task CloseAsync() => await _bus?.StopAsync();
    }
}
