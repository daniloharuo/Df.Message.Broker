using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Df.Message.Broker.ServiceBus.Standard.Contracts
{
    public interface IConsumer
    {
        void RegisterOnMessageHandlerAndReceiveMessages();
    }
}
