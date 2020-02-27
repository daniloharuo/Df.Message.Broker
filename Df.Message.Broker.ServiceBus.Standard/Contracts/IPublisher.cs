using Df.Message.Broker.ServiceBus.Standard.Contracts.Config;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard.Contracts
{
    public interface IPublisher
    {
        void Register<T>(IConfigManager configManager)
                    where T : class;

        Task SendMessagesAsync<T>(T messageObject)
                    where T : class;
    }
}
