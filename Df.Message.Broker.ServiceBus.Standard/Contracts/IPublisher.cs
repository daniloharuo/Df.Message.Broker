using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard.Contracts
{
    public interface IPublisher
    {
        Task SendMessagesAsync<T>(T messageObject) where T : class;
    }
}
