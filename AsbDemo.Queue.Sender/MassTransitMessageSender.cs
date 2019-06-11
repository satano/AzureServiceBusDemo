using AsbDemo.Core;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Sender
{
    class MassTransitMessageSender
    {
        private readonly Options _options;

        public MassTransitMessageSender(Options options)
        {
            _options = options;
        }

        public async Task StartSending()
        {
            var tokenSource = new CancellationTokenSource();
            IBusControl bus = await StartBus();
            var senderTask = Task.Run(() => SendMessagesAsync(bus, tokenSource.Token));

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            tokenSource.Cancel();

            await senderTask;
            await bus.StopAsync();
        }

        private async Task<IBusControl> StartBus()
        {
            Helper.Write("Starting service bus ...");
            var cstrBuilder = new ServiceBusConnectionStringBuilder(Consts.CstrManagement);
            IBusControl bus = Bus.Factory.CreateUsingAzureServiceBus(busCfg =>
            {
                IServiceBusHost host = busCfg.Host(cstrBuilder.Endpoint, hostCfg =>
                {
                    hostCfg.SharedAccessSignature(sasCfg =>
                    {
                        sasCfg.KeyName = cstrBuilder.SasKeyName;
                        sasCfg.SharedAccessKey = cstrBuilder.SasKey;
                    });
                });

                //busCfg.ReceiveEndpoint(host, Consts.MassTransitQueue, receiveCfg =>
                //{
                //    //receiveCfg.Handler<IDemoMessage>(context =>
                //    //{
                //    //    Helper.WriteLine($"Received: {context.Message.Value}");
                //    //    return Task.CompletedTask;
                //    //});
                //});

                busCfg.UseJsonSerializer();
                busCfg.DefaultMessageTimeToLive = TimeSpan.FromMinutes(2);
                busCfg.EnableDeadLetteringOnMessageExpiration = true;
                busCfg.LockDuration = TimeSpan.FromSeconds(30);
                busCfg.AutoDeleteOnIdle = TimeSpan.FromMinutes(10);
                busCfg.MaxDeliveryCount = 10;
                busCfg.EnableDuplicateDetection(TimeSpan.FromMinutes(5));

                //busCfg.Message<IDemoMessage>(msgCfg =>
                //{
                //    _ = msgCfg.GetHashCode();
                //});

                //busCfg.Send<IDemoMessage>(sendCfg =>
                //{
                //});

                //busCfg.ConfigureSend(sendCfg =>
                //{
                //    // Nič moc tu nie je.
                //});

                //busCfg.ConfigurePublish(publishCfg =>
                //{
                //    // Tu je ešte väčšie nič moc.
                //});
            });
            await bus.StartAsync();
            Helper.Write(" started.");
            return bus;
        }

        private async Task SendMessagesAsync(IBusControl bus, CancellationToken token)
        {
            Helper.WriteLine($"Started sending messages.{Environment.NewLine}", ConsoleColor.Magenta);
            while (!token.IsCancellationRequested)
            {
                DemoMessage message = Helper.CreateMessage();
                await bus.Publish<IDemoMessage>(message);

                Helper.WriteLine($"Message sent: Id = {message.Id}", ConsoleColor.Yellow);
                await Task.Delay(_options.ProcessTime);
            }
        }
    }
}
