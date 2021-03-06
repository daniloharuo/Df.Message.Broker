﻿using Df.Message.Broker.Contracts;
using Df.Message.Broker.Contracts.Config;
using Df.Message.Broker.ServiceBus.Standard;
using Df.Message.Broker.ServiceBus.Standard.Config;
using Df.Sample.Model;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Df.Sample
{
    class Program
    {
        private const string _serviceBusConnectionString = "Endpoint=sb://dfmessage.servicebus.windows.net/;SharedAccessKeyName=meu_token;SharedAccessKey=+JWuf875RyazD1Cj8/ezM49LiPk08c+B0lm/I4nqx98=";
        private const string _topicName = "df.magalu.challenge";

        static void Main(string[] args)
        {
            Publisher();
            Consume();
        }

        public static void Consume()
        {
            IConsumer consumer = new ConsumerBuss(();
            IConfigManager configManager = new ConfigManagerBuss((_serviceBusConnectionString, _topicName);
            consumer.Register(configManager);
            consumer.ReceiveMessages<Test>(async (test) => await ProcessEvent(test));
            Console.ReadKey();
        }

        public static async Task ProcessEvent(Test test)
        {
            Console.WriteLine("Event Recived: " + JsonConvert.SerializeObject(test));
            return;
        }

        public static void Publisher()
        {
            Test test = new Test();
            IConfigManager configManager = new ConfigManagerBuss((_serviceBusConnectionString, _topicName);
            IPublisher publisher = new PublisherBuss(();
            publisher.Register<Test>(configManager);
            publisher.SendMessagesAsync(test).GetAwaiter().GetResult();
            Console.WriteLine("Sended!");
        }
    }
}
