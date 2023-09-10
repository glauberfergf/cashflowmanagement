namespace CashFlowManagement.Domain.Interfaces.Infrastructure.RabbitMq
{
    public interface IRabbitMqClient
    {
        Task<bool> SendToQueue(object model);
    }
}
