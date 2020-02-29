using Df.Message.Broker.Contracts;
using Df.Message.Broker.Contracts.Config;
using Jil;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard
{
    public class ConsumerBuss : IConsumer
    {
        private static ISubscriptionClient _subscriptionClient;
        private string _subscriptionName;
        private MessageHandlerOptions _messageHandlerOptions;
        private IConfigManager _configManager;

        public void Register(IConfigManager configManager)
        {
            _subscriptionName = Assembly.GetCallingAssembly().GetName().Name;
            _configManager = configManager;
            CreateTopicSubscriptions().GetAwaiter().GetResult();
            _subscriptionClient = new SubscriptionClient(_configManager.ServiceBusConnectionString, _configManager.TopicName, _subscriptionName);
            Console.WriteLine("Registered with Success");
        }

        public void ReceiveMessages<T>(Func<T, Task> func) where T : class
        {
            _messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = _configManager.MaxConcurrentCalls,
                AutoComplete = _configManager.AutoComplete,
            };

            Func<Microsoft.Azure.ServiceBus.Message, CancellationToken, Task> Input = async (message, cancellationToken) =>
            {
                await ProcessMessagesAsync<T>(message, cancellationToken, func);
            };

            _subscriptionClient.RegisterMessageHandler(Input, _messageHandlerOptions);
        }
        public void ReceiveMessagesGzip<T>(Func<T, Task> func) where T : class
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = _configManager.MaxConcurrentCalls,
                AutoComplete = _configManager.AutoComplete,
            };

            Func<Microsoft.Azure.ServiceBus.Message, CancellationToken, Task> messageHandlerParam = async (message, cancellationToken) =>
            {
                await ProcessMessagesGzipAsync<T>(message, cancellationToken, func);
            };

            _subscriptionClient.RegisterMessageHandler(messageHandlerParam, messageHandlerOptions);

        }

        public async Task ProcessMessagesAsync<T>(Microsoft.Azure.ServiceBus.Message message, CancellationToken cancellationToken, Func<T, Task> task) where T : class
        {
            string messageBody = Encoding.UTF8.GetString(message.Body);

            var result = JSON.Deserialize<T>(messageBody);
            Console.WriteLine($"Received message with Body:{result}");

            await task.Invoke(result);

            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        public async Task ProcessMessagesGzipAsync<T>(Microsoft.Azure.ServiceBus.Message message, CancellationToken cancellationToken, Func<T, Task> task) where T : class
        {
            Stream messageBodyStream = new MemoryStream(message.Body);
            string messageBody = DecompressionStream(messageBodyStream);

            var result = JSON.Deserialize<T>(messageBody);
            Console.WriteLine($"Received message with Body:{result}");

            await task.Invoke(result);

            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;

        }

        private async Task CreateTopicSubscriptions()
        {
            var client = new ManagementClient(_configManager.ServiceBusConnectionString);
            if (!await client.SubscriptionExistsAsync(_configManager.TopicName, _subscriptionName))
            {
                Console.WriteLine($"creating a subscription: {_subscriptionName} in topic: { _configManager.TopicName}");
                await client.CreateSubscriptionAsync(new SubscriptionDescription(_configManager.TopicName, _subscriptionName));
                return;
            }

            Console.WriteLine($"exists subscription: {_subscriptionName} in topic: { _configManager.TopicName}");
        }

        public string DecompressionStream(Stream stream)
        {
            string msgPayload;

            using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                StreamReader reader = new StreamReader(decompressionStream);
                msgPayload = reader.ReadToEnd();
            }
            var json = JSON.Deserialize<JObject>(msgPayload);
            var messageBody = json.Property("Body").Value.ToString();

            return messageBody;
        }
    }
}
