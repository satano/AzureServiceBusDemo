using AsbDemo.Core;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    class MassTransitMessageReceiver : IReceiver
    {
        private readonly Options _options;
        private IBusControl _bus;

        public MassTransitMessageReceiver(Options options)
        {
            _options = options;
        }

        public async Task StartReceivingMessages()
        {
            var tokenSource = new CancellationTokenSource();
            _bus = await Helper.StartBusControl((busCfg, host) =>
            {
                busCfg.ReceiveEndpoint(host, Consts.DemoQueueName, endpointCfg =>
                {
                    endpointCfg.Handler<IDemoMessage>(context =>
                    {
                        Helper.WriteLine($"Message received: {context.Message.Id} | {context.Message.Value}");
                        return Task.Delay(_options.ProcessTime);
                    });
                });
            });
        }

        public async Task CloseAsync() => await _bus.StopAsync();
    }
}
