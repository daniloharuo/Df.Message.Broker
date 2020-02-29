using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Df.Message.Broker.Contracts
{
    public class IExceptionFlow
    {
        public ExceptionReceivedEventArgs exceptionReceivedEventArgs { get;}
    }
}
