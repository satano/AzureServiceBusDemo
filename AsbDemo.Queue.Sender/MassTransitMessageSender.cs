using AsbDemo.Core;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Sender
{
    class MassTransitMessageSender : ISender
    {
        private readonly Options _options;
        private IBusControl _bus;
        private ISendEndpoint _sender;

        public MassTransitMessageSender(Options options)
        {
            _options = options;
        }

        public async Task SendMessagesAsync(CancellationToken token)
        {
            _bus = await Helper.StartBusControl();
            _sender = await _bus.GetSendEndpoint(new Uri(Consts.Endpoint + _options.QueueName));
            Helper.WriteLine("Started sending messages.", ConsoleColor.Magenta);
            while (!token.IsCancellationRequested)
            {
                DemoMessage message = Helper.CreateMessage();
                await _sender.Send<IDemoMessage>(message);

                Helper.WriteLine($"Message sent: Id = {message.Id}", ConsoleColor.Yellow);
                await Task.Delay(_options.ProcessTime);
            }
        }

        public async Task CloseAsync()
        {
            _sender = null;
            await _bus?.StopAsync();
        }
    }
}
