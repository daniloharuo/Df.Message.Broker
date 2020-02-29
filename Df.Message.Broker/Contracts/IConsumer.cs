using Df.Message.Broker.Contracts.Config;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Df.Message.Broker.Contracts
{
    public interface IConsumer
    {
        void Register(IConfigManager configManager);
        void ReceiveMessages<T>(Func<T, Task> func, Func<ExceptionReceivedEventArgs, Task>? exeptionRecived = null) where T : class;
        void ReceiveMessagesGzip<T>(Func<T, Task> func, Func<ExceptionReceivedEventArgs, Task>? exeptionRecived = null) where T : class;
    }
}
