using Df.Message.Broker.Contracts.Config;
using System.Threading.Tasks;

namespace Df.Message.Broker.Contracts
{
    public interface IPublisher
    {
        Task Register<T>(IConfigManager configManager)
                   where T : class;

        Task SendMessagesAsync<T>(T messageObject)
                    where T : class;
    }
}
