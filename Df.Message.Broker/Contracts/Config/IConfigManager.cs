using System;
using System.Collections.Generic;
using System.Text;

namespace Df.Message.Broker.Contracts.Config
{
    public interface IConfigManager
    {
        string ServiceBusConnectionString { get; }
        string TopicName { get; }
        int MaxConcurrentCalls { get; }
        bool AutoComplete { get; }
    }
}
