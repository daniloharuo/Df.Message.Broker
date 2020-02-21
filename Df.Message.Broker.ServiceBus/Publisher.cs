using System;
using System.Text;
using System.Threading.Tasks;
using Df.Message.Broker.ServiceBus.Contracts;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Df.Message.Broker.ServiceBus
{
    public class Publisher<T> : IPublisher<T> where T : class
    {
        private static ITopicClient topicClient;

        public Publisher(string serviceBusConnectionString, string topicName)
        {
            topicClient = new TopicClient(serviceBusConnectionString, topicName);
        }

        public async Task SendMessagesAsync(T messageObject)
        {
            try
            {
                string messageBody = JsonConvert.SerializeObject(messageObject);
                Console.WriteLine(messageBody);
                var message = new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(messageBody));

                await topicClient.SendAsync(message);

            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
            finally
            {
                await topicClient.CloseAsync();
            }
        }
    }

}
