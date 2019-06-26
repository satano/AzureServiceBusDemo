using AsbDemo.Core;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    class MassTransitCommandReceiver : IReceiver
    {
        private readonly Options _options;
        private IBusControl _bus;

        public MassTransitCommandReceiver(Options options)
        {
            _options = options;
        }

        public async Task StartReceivingMessages()
        {
            Helper.WriteLine($"Started receiving messages from queue \"{_options.QueueName}\".", ConsoleColor.Magenta);
            var tokenSource = new CancellationTokenSource();
            _bus = await Helper.StartBusControl((busCfg, host) =>
            {
                busCfg.ReceiveEndpoint(host, _options.QueueName, endpointCfg =>
                {
                    endpointCfg.Handler<IDemoMessage>(context =>
                    {
                        string msg = context.Message.Id + " | " + context.Message.Value;
                        Helper.WriteLine($"Message received: " + msg, ConsoleColor.Yellow);
                        return Task.Delay(_options.ProcessTime);
                    });

                    endpointCfg.Handler<IDemoMessage2>(context =>
                    {
                        string msg = context.Message.Id + " | " + context.Message.Name + " " + context.Message.LastName;
                        Helper.WriteLine($"Message received: " + msg, ConsoleColor.White);
                        return Task.Delay(_options.ProcessTime);
                    });
                });
            });
        }

        public async Task CloseAsync() => await _bus.StopAsync();
    }
}
