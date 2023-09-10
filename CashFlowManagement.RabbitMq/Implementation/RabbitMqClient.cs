using CashFlowManagement.Domain.Interfaces.Infrastructure.RabbitMq;
using CashFlowManagement.Domain.RabbitMq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CashFlowManagement.RabbitMq.Implementation
{
    public class RabbitMqClient : IRabbitMqClient
    {
        private readonly ILogger<RabbitMqClient> _logger;
        private readonly ConnectionFactory _connectionFactory;
        private readonly RabbitMqConfiguration _rabbitMqConfig;

        public RabbitMqClient(IOptions<RabbitMqConfiguration> option, ILogger<RabbitMqClient> logger)
        {
            _rabbitMqConfig = option.Value;
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _rabbitMqConfig.Host
            };
            _logger = logger;
        }

        public async Task<bool> SendToQueue(object model)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ConfirmSelect();
                    channel.ExchangeDeclare(
                        exchange: _rabbitMqConfig.Exchange,  // Nome da exchange da fila principal
                        type: ExchangeType.Topic,
                        durable: true,
                        autoDelete: false,
                        arguments: null
                    );
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    channel.QueueDeclare(
                        queue: _rabbitMqConfig.Queue,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: new Dictionary<string, object>
                        {
                            {"x-dead-letter-exchange", _rabbitMqConfig.ExchangeDLX},  // Nome da troca da DLQ
                            {"x-dead-letter-routing-key", _rabbitMqConfig.RoutingKeyDLQ}  // Roteamento para a DLQ
                        }
                    );

                    // Criar um binding entre a exchange e a fila
                    channel.QueueBind(
                        queue: _rabbitMqConfig.Queue,
                        exchange: _rabbitMqConfig.Exchange,
                        routingKey: _rabbitMqConfig.RoutingKey);

                    var message = JsonSerializer.Serialize(model);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: _rabbitMqConfig.Exchange,
                        routingKey: _rabbitMqConfig.RoutingKey,
                        basicProperties: null,
                        body: body);

                    channel.WaitForConfirmsOrDie();
                }

                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError("RabbitMqClient.SendToQueue: Error to publish message to Queue :{_rabbitMqConfig.Queue}, Message: {e.Message})", _rabbitMqConfig.Queue, e.Message);
            }

            return false;
        }
    }
}
