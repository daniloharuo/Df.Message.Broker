using System;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard.Contracts
{
    public interface IConsumer
    {
        void Register(string serviceBusConnectionString, string topicName);
        void ReceiveMessages<T>(Func<T, Task> func) where T : class;
        void ReceiveMessagesGzip<T>(Func<T, Task> func) where T : class;
    }
}
