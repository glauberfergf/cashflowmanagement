namespace CashFlowManagement.Domain.RabbitMq
{
    public class RabbitMqConfiguration
    {
        public string Host { get; set; } = null!;
        public string Exchange { get; set; } = null!;
        public string Queue { get; set; } = null!;
        public string RoutingKey { get; set; } = null!;
        public string ExchangeDLX { get; set; } = null!;
        public string QueueDLQ { get; set; } = null!;
        public string RoutingKeyDLQ { get; set; } = null!;
        //public string UserName { get; set; } = null!;
        //public string Password { get; set; } = null!;
    }
}
