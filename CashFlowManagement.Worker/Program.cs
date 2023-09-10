using CashFlowManagement.CrossCutting;
using CashFlowManagement.Domain.Configuration;
using CashFlowManagement.Domain.RabbitMq;
using CashFlowManagement.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<RabbitMqConfiguration>(hostContext.Configuration.GetSection("RabbitMqConfig"));

        services.Configure<DbSettings>(hostContext.Configuration.GetSection("DbSettings"));

        //DI
        services.RegisterServices();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
