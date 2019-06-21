using System.Threading.Tasks;

namespace AsbDemo.Topic.Receiver
{
    interface IReceiver
    {
        Task StartReceivingMessages();
        Task CloseAsync();
    }
}
