using System.Threading.Tasks;
using MyApp.ServiceModel;
using NUnit.Framework;
using ServiceStack.Messaging;
using ServiceStack.Azure.Messaging;

namespace MyApp.Tests
{
    public class MqTest
    {
        private ServiceBusMqServer MqServer;
        private readonly IMessageFactory MqFactory;
        public MqTest()
        {
            MqServer = new ServiceBusMqServer("<ServiceBus ConnectionString>") {
                DisablePublishingToOutq = true,
            };
            MqFactory = MqServer.MessageFactory;
        }

        [Test] // requires running Host MQ Server project
        public void Can_send_Request_Reply_message()
        {
            using (var mqClient = MqFactory.CreateMessageQueueClient())
            {
                var replyToMq = mqClient.GetTempQueueName();

                mqClient.Publish(new Message<Hello>(new Hello { Name = "MQ Worker" })
                {
                    ReplyTo = replyToMq,
                });

                var responseMsg = mqClient.Get<HelloResponse>(replyToMq);
                mqClient.Ack(responseMsg);
                Assert.That(responseMsg.GetBody().Result, Is.EqualTo("Hello, MQ Worker!"));
            }
        }
    }
}