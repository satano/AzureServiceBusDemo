using AsbDemo.Core;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Sender
{
    class MassTransitCommandSender : ISender
    {
        private readonly Options _options;
        private IBusControl _bus;
        private ISendEndpoint _sender;

        public MassTransitCommandSender(Options options)
        {
            _options = options;
        }

        public async Task SendMessagesAsync(CancellationToken token)
        {
            _bus = await Helper.StartBusControl();
            _sender = await _bus.GetSendEndpoint(new Uri(Consts.Endpoint + _options.QueueName));
            Helper.WriteLine($"Started sending messages to queue \"{_options.QueueName}\".", ConsoleColor.Magenta);
            bool useMessage2 = false;
            while (!token.IsCancellationRequested)
            {
                ConsoleColor color;
                IMessageBase message;
                if (useMessage2)
                {
                    message = Helper.CreateMessage2();
                    await _sender.Send<IDemoMessage2>(message);
                    color = ConsoleColor.White;
                }
                else
                {
                    message = Helper.CreateMessage();
                    await _sender.Send<IDemoMessage>(message);
                    color = ConsoleColor.Yellow;
                }
                Helper.WriteLine($"Sent message {message.GetType().Name}: Id = {message.Id}", color);

                useMessage2 = !useMessage2;
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
