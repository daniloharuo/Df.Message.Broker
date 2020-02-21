using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Df.Message.Broker.ServiceBus
{
    public class Publisher
    {
        private string _serviceBusConnectionString = "Endpoint=sb://dfmessage.servicebus.windows.net/;SharedAccessKeyName=meu_token;SharedAccessKey=+JWuf875RyazD1Cj8/ezM49LiPk08c+B0lm/I4nqx98=";
        private string _topicName = "df.magalu.challenge";
        private static ITopicClient topicClient;

        public Publisher(string serviceBusConnectionString, string topicName)
        {
            topicClient = new TopicClient(_serviceBusConnectionString, _topicName);
        }

        public async Task StartTest()
        {

            var objectTest = new { Version = Environment.Version, ProjectName = Assembly.GetCallingAssembly().GetName().Name };
            await SendMessagesAsync(objectTest);

        }

        public async Task SendMessagesAsync(object messageObject)
        {
            try
            {
                string messageBody = JsonConvert.SerializeObject(messageObject);
                Console.WriteLine(messageBody);
                var message = new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(messageBody));

                await topicClient.SendAsync(message);

            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
            finally
            {
                await topicClient.CloseAsync();
            }
        }
    }
}
