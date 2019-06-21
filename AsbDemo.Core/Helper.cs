using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Core
{
    public static class Helper
    {
        public const string PriorityKey = "Priority";

        private static int _messageCounter = 0;


        public static Message CreateAzureMessage(TimeSpan? timeToLive = null)
        {
            DemoMessage demoMsg = CreateMessage();
            return new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(demoMsg)))
            {
                ContentType = "application/json",
                MessageId = demoMsg.Id,
                TimeToLive = timeToLive ?? TimeSpan.FromMinutes(2)
            };
        }

        public static DemoMessage CreateMessage()
        {
            int currentCounter = Interlocked.Increment(ref _messageCounter);
            return new DemoMessage()
            {
                Id = currentCounter.ToString(),
                Value = $"Lorem ipsum - {currentCounter}"
            };
        }

        public static void WriteLine(string message)
        {
            lock (Console.Out)
            {
                Console.WriteLine(message);
            }
        }

        public static void WriteLine(string message, ConsoleColor color)
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public static void Write(string message)
        {
            lock (Console.Out)
            {
                Console.Write(message);
            }
        }

        public static void Write(string message, ConsoleColor color)
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = color;
                Console.Write(message);
                Console.ResetColor();
            }
        }

        public static async Task<IBusControl> StartBusControl(
            Action<IServiceBusBusFactoryConfigurator, IServiceBusHost> configurator = null)
        {
            Write("Starting service bus ...", ConsoleColor.Magenta);
            var cstrBuilder = new ServiceBusConnectionStringBuilder(Consts.CstrManagement);
            IBusControl bus = Bus.Factory.CreateUsingAzureServiceBus(busCfg =>
            {
                IServiceBusHost host = busCfg.Host(cstrBuilder.Endpoint, hostCfg =>
                {
                    hostCfg.SharedAccessSignature(sasCfg =>
                    {
                        sasCfg.KeyName = cstrBuilder.SasKeyName;
                        sasCfg.SharedAccessKey = cstrBuilder.SasKey;
                        sasCfg.TokenTimeToLive = TimeSpan.FromMinutes(1);
                    });
                });

                busCfg.UseJsonSerializer();
                busCfg.DefaultMessageTimeToLive = TimeSpan.FromMinutes(2);
                busCfg.EnableDeadLetteringOnMessageExpiration = true;
                busCfg.LockDuration = TimeSpan.FromSeconds(30);
                busCfg.AutoDeleteOnIdle = TimeSpan.FromMinutes(10);
                busCfg.MaxDeliveryCount = 10;
                busCfg.EnableDuplicateDetection(TimeSpan.FromMinutes(5));

                configurator?.Invoke(busCfg, host);
            });
            await bus.StartAsync();
            WriteLine(" started.", ConsoleColor.Magenta);

            return bus;
        }
    }
}
