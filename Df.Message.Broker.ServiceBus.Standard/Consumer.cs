using Df.Message.Broker.ServiceBus.Standard.Contracts;
using Jil;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
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
    public class Consumer : IConsumer
    {
        private static ISubscriptionClient _subscriptionClient;
        private string _subscriptionName;

        public Consumer()
        {
            Console.WriteLine("version: " + "0.08");
        }

        public void Register(string serviceBusConnectionString, string topicName)
        {
            _subscriptionName = Assembly.GetCallingAssembly().GetName().Name;
            CreateTopicSubscriptions(serviceBusConnectionString, topicName).GetAwaiter().GetResult();
            _subscriptionClient = new SubscriptionClient(serviceBusConnectionString, topicName, _subscriptionName);
        }

        public void ReceiveMessages<T>(Func<T, Task> func) where T : class
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false,
            };

            Func<Microsoft.Azure.ServiceBus.Message, CancellationToken, Task> Input = async (message, cancellationToken) =>
            {
                await ProcessMessagesAsync<T>(message, cancellationToken, func);
            };

            _subscriptionClient.RegisterMessageHandler(Input, messageHandlerOptions);
        }
        public void ReceiveMessagesGzip<T>(Func<T, Task> func) where T : class
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false,
            };

            Func<Microsoft.Azure.ServiceBus.Message, CancellationToken, Task> Input = async (message, cancellationToken) =>
            {
                await ProcessMessagesGzipAsync<T>(message, cancellationToken, func);
            };

            _subscriptionClient.RegisterMessageHandler(Input, messageHandlerOptions);
        }

        public async Task ProcessMessagesAsync<T>(Microsoft.Azure.ServiceBus.Message message, CancellationToken cancellationToken, Func<T, Task> task) where T : class
        {
            string messageRecipt = Encoding.UTF8.GetString(message.Body);

            using (var input = new StringReader(messageRecipt))
            {
                var result = JSON.Deserialize<T>(input);
                Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{messageRecipt}");
                await task.Invoke(result);
            }

            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        public async Task ProcessMessagesGzipAsync<T>(Microsoft.Azure.ServiceBus.Message message, CancellationToken cancellationToken, Func<T, Task> task) where T : class
        {

            Stream stream = new MemoryStream(message.Body);
            string msgPayload;

            using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                StreamReader reader = new StreamReader(decompressionStream);
                msgPayload = reader.ReadToEnd();
            }

            using (var input = new StringReader(msgPayload))
            {
                var json = JSON.Deserialize<JObject>(msgPayload);
                var propertyValue = json.Property("Body").Value;
                var result = JSON.Deserialize<T>(propertyValue.ToString());

                Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{input}");
                await task.Invoke(result);
            }

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

        private async Task CreateTopicSubscriptions(string serviceBusConnectionString, string topicName)
        {
            var client = new ManagementClient(serviceBusConnectionString);
            if (!await client.SubscriptionExistsAsync(topicName, _subscriptionName))
            {
                Console.WriteLine($"creating a subscription: {_subscriptionName} in topic: {topicName}");
                await client.CreateSubscriptionAsync(new SubscriptionDescription(topicName, _subscriptionName));
                return;
            }

            Console.WriteLine($"exists subscription: {_subscriptionName} in topic: {topicName}");
        }
    }
}
