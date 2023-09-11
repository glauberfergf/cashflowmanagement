using Moq;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using CashFlowManagement.Domain.RabbitMq;
using CashFlowManagement.RabbitMq.Implementation;

namespace CashFlowManagement.Test.Infrastructure.RabbitMq
{
    [TestFixture]
    public class RabbitMqClientTests
    {
        [Test]
        public async Task SendToQueue_Should_Succeed()
        {
            // Arrange
            var rabbitMqConfig = new RabbitMqConfiguration
            {
                Host = "localhost", // Configure as needed
                Exchange = "CashFlowManagement",
                Queue = "OrderPaymentCreated",
                RoutingKey = "new",
                ExchangeDLX = "CashFlowManagement_DLX",
                QueueDLQ = "OrderPaymentCreated_DLQ",
                RoutingKeyDLQ = "new_dlx"
            };

            var options = Options.Create(rabbitMqConfig);

            var loggerMock = new Mock<ILogger<RabbitMqClient>>();
            var connectionFactoryMock = new Mock<IConnectionFactory>();
            var connectionMock = new Mock<IConnection>();
            var modelMock = new Mock<IModel>();

            connectionFactoryMock.Setup(factory => factory.CreateConnection()).Returns(connectionMock.Object);
            connectionMock.Setup(connection => connection.CreateModel()).Returns(modelMock.Object);

            var client = new RabbitMqClient(options, loggerMock.Object);
            var message = new { Message = "Test Message" };

            // Act
            var result = await client.SendToQueue(message);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}