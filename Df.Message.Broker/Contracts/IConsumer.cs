using Df.Message.Broker.Contracts.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Df.Message.Broker.Contracts
{
    public interface IConsumer
    {
        void Register(IConfigManager configManager);
        void ReceiveMessages<T>(Func<T, Task> func) where T : class;
        void ReceiveMessagesGzip<T>(Func<T, Task> func) where T : class;
    }
}
