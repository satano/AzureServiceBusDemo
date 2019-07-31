using AsbDemo.Core;
using Kros.MassTransit.AzureServiceBus;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    class MassTransitCommandFluentReceiver : IReceiver
    {
        private readonly Options _options;
        private IBusControl _bus;

        public MassTransitCommandFluentReceiver(Options options)
        {
            _options = options;
        }

        public async Task StartReceivingMessages()
        {
            Helper.Write("Starting service bus ...", ConsoleColor.Magenta);

            _bus = new MassTransitForAzureBuilder(_options.ConnectionString)
                .ConfigureServiceBusFactory((busCfg, host) =>
                {
                    Helper.WriteLine($"Started receiving messages from queue \"{_options.QueueName}\".", ConsoleColor.Magenta);
                })
                .ConfigureQueue(_options.QueueName)
                    .AddConsumer<IDemoMessage>(context =>
                    {
                        string msg = context.Message.Id + " | " + context.Message.Value;
                        Helper.WriteLine($"Message received: " + msg, ConsoleColor.Yellow);
                        return Task.Delay(_options.ProcessTime);
                    })
                    .AddConsumer<IDemoMessage2>(context =>
                    {
                        string msg = context.Message.Id + " | " + context.Message.Name + " " + context.Message.LastName;
                        Helper.WriteLine($"Message received: " + msg, ConsoleColor.White);
                        return Task.Delay(_options.ProcessTime);
                    })
                .Build();
            await _bus.StartAsync();

            Helper.WriteLine(" started.", ConsoleColor.Magenta);
        }

        public async Task CloseAsync() => await _bus.StopAsync();
    }
}
