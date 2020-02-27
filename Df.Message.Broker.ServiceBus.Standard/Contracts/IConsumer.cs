using Df.Message.Broker.ServiceBus.Standard.Contracts.Config;
using System;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard.Contracts
{
    public interface IConsumer
    {
        void Register(IConfigManager configManager);
        void ReceiveMessages<T>(Func<T, Task> func) where T : class;
        void ReceiveMessagesGzip<T>(Func<T, Task> func) where T : class;
    }
}
