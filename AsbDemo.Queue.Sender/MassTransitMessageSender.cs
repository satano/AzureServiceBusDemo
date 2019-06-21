using AsbDemo.Core;
using MassTransit;
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
            IBusControl bus = await Helper.StartBusControl();
            var senderTask = Task.Run(() => SendMessagesAsync(bus, tokenSource.Token));

            Console.ReadLine();
            Helper.WriteLine("Finishing...", ConsoleColor.Green);
            tokenSource.Cancel();

            await senderTask;
            await bus.StopAsync();
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
