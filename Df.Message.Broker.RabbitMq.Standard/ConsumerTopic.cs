using Df.Message.Broker.Contracts;
using Df.Message.Broker.Contracts.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Df.Message.Broker.RabbitMq.Standard
{
    public class ConsumerTopic : IConsumer
    {
        public void ReceiveMessages<T>(Func<T, Task> func, Func<Microsoft.Azure.ServiceBus.ExceptionReceivedEventArgs, Task> exeptionRecived = null) where T : class
        {
            throw new NotImplementedException();
        }

        public void ReceiveMessagesGzip<T>(Func<T, Task> func, Func<Microsoft.Azure.ServiceBus.ExceptionReceivedEventArgs, Task> exeptionRecived = null) where T : class
        {
            throw new NotImplementedException();
        }

        public void Register(IConfigManager configManager)
        {
            throw new NotImplementedException();
        }
    }
}
