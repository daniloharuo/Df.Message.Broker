using Df.Message.Broker.ServiceBus.Standard.Contracts;
using Df.Message.Broker.ServiceBus.Standard.Contracts.Config;
using Jil;
using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard
{
    public class PublisherBuss : IPublisher
    {
        private static ITopicClient topicClient;
        private IConfigManager _configManager;
        private Dictionary<Type, ITopicClient> topicClients;

        public PublisherBuss()
        {
            topicClients = new Dictionary<Type, ITopicClient>();
        }
        public void Register<T>(IConfigManager configManager) where T : class
        {
            _configManager = configManager;
            CreateTopic();
            topicClient = new TopicClient(configManager.ServiceBusConnectionString, configManager.TopicName);

            if (!topicClients.ContainsKey(typeof(T)))
            {
                topicClients.Add(typeof(T), topicClient);
            }
        }

        public async Task SendMessagesAsync<T>(T messageObject)
             where T : class
        {
            try
            {
                ValidateRegister<T>();

                string messageBody = JSON.SerializeDynamic(messageObject);

                Console.WriteLine(messageBody);

                var message = new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(messageBody));

                await topicClients[typeof(T)].SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {ex.Message}");
                throw ex;
            }
        }

        public void ValidateRegister<T>() where T : class
        {
            if (!topicClients.ContainsKey(typeof(T)))
            {
                throw new Exception("Topic was not registered");
            }
        }

        public void CreateTopic()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_configManager.ServiceBusConnectionString);

            if (!namespaceManager.TopicExists(_configManager.TopicName))
            {
                namespaceManager.CreateTopic(_configManager.TopicName);
                Console.WriteLine($"creating a topic:{ _configManager.TopicName}");
                return;
            }

            Console.WriteLine($"exists topic: { _configManager.TopicName}");
        }
    }
}