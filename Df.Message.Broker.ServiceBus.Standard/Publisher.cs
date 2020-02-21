using Df.Message.Broker.ServiceBus.Standard.Contracts;
using Jil;
using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard
{
    public class Publisher  : IPublisher
    {
        private static ITopicClient topicClient;

        public Publisher(string serviceBusConnectionString, string topicName)
        {
            topicClient = new TopicClient(serviceBusConnectionString, topicName);
        }

        public async Task SendMessagesAsync<T>(T messageObject)
             where T : class
        {
            try
            {
                string messageBody = JSON.SerializeDynamic(messageObject);

                Console.WriteLine(messageBody);

                var message = new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(messageBody));

                await topicClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}