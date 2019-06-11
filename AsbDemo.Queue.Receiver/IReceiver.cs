using System.Threading.Tasks;

namespace AsbDemo.Queue.Receiver
{
    internal interface IReceiver
    {
        Task CloseAsync();
    }
}
