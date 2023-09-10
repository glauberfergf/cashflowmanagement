
using CashFlowManagement.AntiCorruption.Facades;
using CashFlowManagement.AntiCorruption.Gateways;
using CashFlowManagement.Application.Implementation;
using CashFlowManagement.Domain.Interfaces.Application;
using CashFlowManagement.Domain.Interfaces.Infrastructure.RabbitMq;
using CashFlowManagement.Domain.Interfaces.Infrastructure.Repository;
using CashFlowManagement.Domain.Services;
using CashFlowManagement.RabbitMq.Implementation;
using CashFlowManagement.Repository.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlowManagement.CrossCutting
{
    public static class NativeInjectorConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            //Application Layer
            services.AddSingleton<IPaymentApplication, PaymentApplication>();

            //Infrastructure  Layer
            services.AddSingleton<IPaymentRepository, PaymentRepository>();
            services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();

            //Services Layer
            //Facades
            services.AddSingleton<ICreditCardFacade, CreditCardFacade>();
            services.AddSingleton<IDebitCardFacade, DebitCardFacade>();

            //Services Layer
            //Gateways
            services.AddSingleton<IPayPalGateway, PayPalGateway>();
            services.AddSingleton<IEloGateway, EloGateway>();
        }
    }
}
