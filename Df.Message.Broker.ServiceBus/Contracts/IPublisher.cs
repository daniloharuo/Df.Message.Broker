using System;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Contracts
{

    public interface IPublisher<T> where T : class
    {
        Task SendMessagesAsync(T messageObject);
    }
}