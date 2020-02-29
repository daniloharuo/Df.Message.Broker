using Df.Message.Broker.Contracts.Config;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard.Config
{
    public sealed class ConfigManagerBus : IConfigManager
    {
        public ConfigManagerBus(
            string serviceBusConnectionString,
            string topicName,
            int maxConcurrentCalls = 1,
            bool autoComplete = false
            )
        {
            ServiceBusConnectionString = serviceBusConnectionString;
            TopicName = topicName;
            MaxConcurrentCalls = maxConcurrentCalls;
            AutoComplete = autoComplete;
        }


        public string ServiceBusConnectionString { get; private set; }
        public string TopicName { get; private set; }
        public int MaxConcurrentCalls { get; private set; }
        public bool AutoComplete { get; private set; }
        public Func<ExceptionReceivedEventArgs, Task> ExceptionReceived { get; private set; }
    }
}
