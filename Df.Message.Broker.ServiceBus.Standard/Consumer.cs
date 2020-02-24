using Df.Message.Broker.ServiceBus.Standard.Contracts;
using Jil;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Df.Message.Broker.ServiceBus.Standard
{
    public class Consumer //: IConsumer
    {
        private static ISubscriptionClient _subscriptionClient;
        private readonly string _subscriptionName = Assembly.GetCallingAssembly().GetName().Name;

        public Consumer(string serviceBusConnectionString, string topicName)
        {
            CreateTopicSubscriptions(serviceBusConnectionString, topicName).GetAwaiter().GetResult();
            _subscriptionClient = new SubscriptionClient(serviceBusConnectionString, topicName, _subscriptionName);
        }

        public void RegisterOnMessageHandlerAndReceiveMessages(Func<Task> func)
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false,
            };
            Func<Microsoft.Azure.ServiceBus.Message, CancellationToken, Task> Input  = null;
            Func<ExceptionReceivedEventArgs, System.Threading.Tasks.Task> ErrorInPut = null;

            _subscriptionClient.RegisterMessageHandler(Input,ErrorInPut);         
        }
        public async Task ProcessMessagesAsync(Microsoft.Azure.ServiceBus.Message message, CancellationToken cancellationToken, Task task)
        {

            string messageRecipt = Encoding.UTF8.GetString(message.Body);

            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{messageRecipt}");


            //service.process(param);
            //var DynamicObject = JSON.Deserialize<T>(messageRecipt);

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
