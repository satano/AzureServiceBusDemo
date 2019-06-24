using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    interface IReceiver
    {
        Task StartReceivingMessages();
        Task CloseAsync();
    }
}
