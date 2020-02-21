using System;
using System.Reflection;
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
                Teste teste = new Teste();
                var publisher = new Publisher<Teste>(serviceBusConnectionString, topicName);
                publisher.SendMessagesAsync(teste).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public class Teste
        {
            public string Version{get;private set;}
            public string ProjectName {get;private set;}
            public Teste()
            {
                Version = Environment.Version.ToString();
                ProjectName = Assembly.GetCallingAssembly().GetName().Name;
            }    
        }    

    }
}
