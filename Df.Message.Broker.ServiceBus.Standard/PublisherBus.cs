using Df.Message.Broker.Contracts;
using Df.Message.Broker.Contracts.Config;
using Jil;
using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard
{
    public class PublisherBus : IPublisher
    {
        private static ITopicClient _topicClient;
        private IConfigManager _configManager;
        private Dictionary<Type, ITopicClient> _topicClients;

        public PublisherBus()
        {
            _topicClients = new Dictionary<Type, ITopicClient>();
        }

        public void Register<T>(IConfigManager configManager) where T : class
        {
            _configManager = configManager;
            CreateTopic();
            _topicClient = new TopicClient(configManager.ServiceBusConnectionString, configManager.TopicName);

            if (!_topicClients.ContainsKey(typeof(T)))
            {
                _topicClients.Add(typeof(T), _topicClient);
            }
        }

        public async Task SendMessagesAsync<T>(T messageObject)
             where T : class
        {
            try
            {
                ValidateRegister<T>();

                string messageBody = JSON.SerializeDynamic(messageObject);

                Console.WriteLine($"Body message is: {messageBody}");

                var message = new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(messageBody));

                await _topicClients[typeof(T)].SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {ex.Message}");
                throw ex;
            }
        }

        public void ValidateRegister<T>() where T : class
        {
            if (!_topicClients.ContainsKey(typeof(T)))
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