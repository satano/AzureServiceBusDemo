using System.Threading;
using System.Threading.Tasks;

namespace AsbDemo.Queue.Sender
{
    interface ISender
    {
        Task SendMessagesAsync(CancellationToken token);
        Task CloseAsync();
    }
}
