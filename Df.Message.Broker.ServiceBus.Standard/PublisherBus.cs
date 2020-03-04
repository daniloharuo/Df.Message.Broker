using Df.Message.Broker.Contracts;
using Df.Message.Broker.Contracts.Config;
using Jil;
using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceBus;
using Polly;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard
{
    public class PublisherBus : IPublisher
    {
        private static ITopicClient _topicClient;
        private IConfigManager _configManager;
        private Dictionary<Type, ITopicClient> _topicClients;
        private const int _fiveRetryCount = 5;
        private const int _threeHundredSeconds = 300;

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

                var policy = Policy.Handle<Exception>()
                   .WaitAndRetry(_fiveRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                   {
                      //Send message to dead 
                   }
               );

                //var retry = Policy.Handle<Exception>()
                //            .WaitAndRetry(_fiveRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                //var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreaker(3, TimeSpan.FromSeconds(15), onBreak: (ex, timespan, context) =>
                //{
                //    Console.WriteLine("Circuito entrou em estado de falha");
                //}, onReset: (context) =>
                //{
                //    Console.WriteLine("Circuito saiu do estado de falha");
                //});
                //await retry.Execute(async () =>
                //   await circuitBreakerPolicy.Execute(async () =>

                //            await _topicClients[typeof(T)].SendAsync(message)
                //));

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
            var _subscriptionName = Assembly.GetCallingAssembly().GetName().Name;
            string topic = _subscriptionName.Replace('.', '/') + _configManager.TopicName;

            if (!namespaceManager.TopicExists(topic))
            {
                namespaceManager.CreateTopic(_configManager.TopicName);
                Console.WriteLine($"creating a topic:{ _configManager.TopicName}");
                return;
            }

            Console.WriteLine($"exists topic: { _configManager.TopicName}");
        }
    }
}