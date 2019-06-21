using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Topic.Sender
{
    interface ISender
    {
        Task SendMessagesAsync(CancellationToken token);
        Task CloseAsync();
    }
}
