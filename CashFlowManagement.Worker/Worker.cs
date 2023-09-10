using CashFlowManagement.Domain.Entity;
using CashFlowManagement.Domain.Interfaces.Application;
using CashFlowManagement.Domain.Interfaces.Infrastructure.RabbitMq;
using CashFlowManagement.Domain.RabbitMq;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CashFlowManagement.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqConfiguration _rabbitMqConfig;
        
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IPaymentApplication _paymentApplication;
        private readonly IRabbitMqConsumer _rabbitMqConsumer;

        private readonly IModel _channel;

        public Worker(IOptions<RabbitMqConfiguration> option, IRabbitMqConsumer rabbitMqConsumer, IHostApplicationLifetime applicationLifetime, IPaymentApplication paymentApplication, ILogger<Worker> logger)
        {
            _logger = logger;
            _rabbitMqConfig = option.Value;
            _rabbitMqConsumer = rabbitMqConsumer;
            _channel = _rabbitMqConsumer.InitializeQueues().Result;
            _paymentApplication = paymentApplication;
            _applicationLifetime = applicationLifetime;            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("CashFlowManagement Worker running at: {time}", DateTimeOffset.Now);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (sender, eventArgs) =>
                {
                    var contentArray = eventArgs.Body.ToArray();
                    var contentString = Encoding.UTF8.GetString(contentArray);

                    Payment? payment = JsonSerializer.Deserialize<Payment>(contentString);

                    if (payment is not null)
                        _paymentApplication.CreateAsync(payment);
                    else
                    {
                        _logger.LogError($"CashFlowManagement.Worker - Received Payment null");
                    }

                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                };

                _channel.BasicConsume(_rabbitMqConfig.Queue, false, consumer);
            }
            catch (Exception e)
            {
                _logger.LogError($"CashFlowManagement.Worker - Error: ", e.Message);
            }

             await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CashFlowManagement Worker is stopped");

            //Command to terminate application in pod
            _applicationLifetime.StopApplication();
            
            return base.StopAsync(cancellationToken);
        }
    }
}