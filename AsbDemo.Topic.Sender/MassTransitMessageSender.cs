using AsbDemo.Core;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Topic.Sender
{
    class MassTransitMessageSender : ISender
    {
        private readonly Options _options;
        private IBusControl _bus;

        public MassTransitMessageSender(Options options)
        {
            _options = options;
        }

        public async Task SendMessagesAsync(CancellationToken token)
        {
            _bus = await Helper.StartBusControl();
            Helper.WriteLine("Started sending messages.", ConsoleColor.Magenta);
            while (!token.IsCancellationRequested)
            {
                Priority priority = AzureTopicSender.GetPriority();
                IDemoMessage message = Helper.CreateMessage();
                await _bus.Publish<IDemoMessage>(message, ctx =>
                {
                    ctx.TimeToLive = Consts.DefaultMessageTimeToLive;
                    ctx.Headers.Set(Helper.PriorityKey, priority.ToString());
                });
                Helper.WriteLine($"Message sent: Id = {message.Id}, Priority = {priority}", ConsoleColor.Yellow);
                await Task.Delay(_options.ProcessTime);
            }
        }

        public async Task CloseAsync() => await _bus?.StopAsync();
    }
}
