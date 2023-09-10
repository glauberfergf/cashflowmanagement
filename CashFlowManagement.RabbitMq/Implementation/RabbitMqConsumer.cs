using CashFlowManagement.Domain.Interfaces.Infrastructure.RabbitMq;
using CashFlowManagement.Domain.RabbitMq;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CashFlowManagement.RabbitMq.Implementation
{
    public class RabbitMqConsumer : IRabbitMqConsumer
    {
        private readonly RabbitMqConfiguration _rabbitMqConfig;
        private readonly ConnectionFactory _factory;

        private readonly IModel _channel;
        private readonly IConnection _connection;

        public RabbitMqConsumer(IOptions<RabbitMqConfiguration> option)
        {
            _rabbitMqConfig = option.Value;
            _factory = new ConnectionFactory
            {
                HostName = _rabbitMqConfig.Host
                // Adicione outras configurações necessárias aqui
            };

            // Configuração da DLQ
            //using var connection = _factory.CreateConnection();
            //using var channel = connection.CreateModel();
            //channel.ExchangeDeclare(_config.ExchangeDLX, ExchangeType.Direct, durable: true,autoDelete: false,arguments: null);
            //channel.QueueDeclare(_config.QueueDLQ, durable: true, exclusive: false, autoDelete: false, arguments: null);
            //channel.QueueBind(_config.QueueDLQ, _config.ExchangeDLX, _config.RoutingKeyDLQ);

            _connection = CreateConnection();
            _channel = _connection.CreateModel();
        }

        public IConnection CreateConnection()
        {
            return _factory.CreateConnection();
        }

        public Task<IModel> InitializeQueues()
        {            
            _channel.ExchangeDeclare(
                        exchange: _rabbitMqConfig.Exchange,  // Nome da exchange da fila principal
                        type: ExchangeType.Topic,
                        durable: true,
                        autoDelete: false,
                        arguments: null
                    );
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            _channel.QueueDeclare(
                queue: _rabbitMqConfig.Queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                        {
                            {"x-dead-letter-exchange", _rabbitMqConfig.ExchangeDLX},  // Nome da troca da DLQ
                            {"x-dead-letter-routing-key", _rabbitMqConfig.RoutingKeyDLQ}  // Roteamento para a DLQ
                        });
            _channel.QueueBind(
                        queue: _rabbitMqConfig.Queue,
                        exchange: _rabbitMqConfig.Exchange,
                        routingKey: _rabbitMqConfig.RoutingKey);

            return Task.FromResult(_channel);
        }
    }
}
