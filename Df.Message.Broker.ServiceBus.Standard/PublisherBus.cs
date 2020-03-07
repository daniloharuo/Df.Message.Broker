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
        private string _topicName;
        private string _deadLetterName;
        private NamespaceManager _namespaceManager;
        private QueueClient _queueClient;

        public PublisherBus()
        {
            _topicClients = new Dictionary<Type, ITopicClient>();
        }

        public async Task Register<T>(IConfigManager configManager) where T : class
        {
            _configManager = configManager;
            await CreateTopicAsync();

            _topicClient = new TopicClient(configManager.ServiceBusConnectionString, _topicName);
            _queueClient = new QueueClient(configManager.ServiceBusConnectionString, _deadLetterName);

            if (!_topicClients.ContainsKey(typeof(T)))
            {
                _topicClients.Add(typeof(T), _topicClient);
            }
        }
        public async Task SendMessagesAsync<T>(T messageObject)
             where T : class
        {
            ValidateRegister<T>();

            string messageBody = JSON.SerializeDynamic(messageObject);

            Console.WriteLine($"Body message is: {messageBody}");

            var message = new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(messageBody));

            var policy = Policy.Handle<Exception>()
                .WaitAndRetry(_fiveRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _queueClient.SendAsync(message);
                }
            );

            await policy.Execute(async () =>
                await _topicClients[typeof(T)].SendAsync(message)
            );
        }

        private void ValidateRegister<T>() where T : class
        {
            if (!_topicClients.ContainsKey(typeof(T)))
            {
                throw new Exception("Topic was not registered");
            }
        }

        private async Task CreateTopicAsync()
        {
           _namespaceManager = NamespaceManager.CreateFromConnectionString(_configManager.ServiceBusConnectionString);
            var _subscriptionName = Assembly.GetCallingAssembly().GetName().Name;
            
            _topicName = _subscriptionName.Replace('.', '/') + _configManager.TopicName;
            await CreateDeadLetterQueueAsync();
            if (!_namespaceManager.TopicExists(_topicName))
            {
                await _namespaceManager.CreateTopicAsync(_topicName);
                Console.WriteLine($"creating a topic:{ _topicName}");
                return;
            }

            Console.WriteLine($"exists topic: {_topicName}");
        }

        private async Task CreateDeadLetterQueueAsync()
        {
            _deadLetterName = _topicName+ "_DL";

            if (!_namespaceManager.QueueExists(_deadLetterName))
            {
                await _namespaceManager.CreateQueueAsync(_deadLetterName);
                Console.WriteLine($"creating a dead letter queue:{ _deadLetterName}");
                return;
            }
            Console.WriteLine($"exists dead letter queue: {_deadLetterName}");
        }
    }
}