using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Df.Message.Broker.Contracts
{
    public interface IExceptionFlow
    {
        ExceptionReceivedEventArgs exceptionReceivedEventArgs { get;}
    }
}
