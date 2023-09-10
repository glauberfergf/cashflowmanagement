using RabbitMQ.Client;

namespace CashFlowManagement.Domain.Interfaces.Infrastructure.RabbitMq
{
    public interface IRabbitMqConsumer
    {
        IConnection CreateConnection();
        Task<IModel> InitializeQueues();
    }
}
