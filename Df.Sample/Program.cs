using System;
using Df.Message.Broker.ServiceBus;

namespace Df.Sample
{
    class Program
    {
        const string serviceBusConnectionString = "Endpoint=sb://dfmessage.servicebus.windows.net/;SharedAccessKeyName=meu_token;SharedAccessKey=+JWuf875RyazD1Cj8/ezM49LiPk08c+B0lm/I4nqx98=";
        const string topicName = "df.magalu.challenge";
        static void Main(string[] args)
        {

            Consume();

        }


        public static void Consume()
        {

            Consumer consumer = new Consumer(serviceBusConnectionString, topicName);
            try
            {
                consumer.RegisterOnMessageHandlerAndReceiveMessages();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public static void Publisher()
        {
            try
            {
                Publisher publisher = new Publisher(serviceBusConnectionString, topicName);
                publisher.StartTest().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
