namespace Df.Message.Broker.ServiceBus.Standard.Contracts.Config
{
    public interface IConfigManager
    {
        string ServiceBusConnectionString { get; }
        string TopicName { get; }
        int MaxConcurrentCalls { get; }
        bool AutoComplete { get; }
    }
}
