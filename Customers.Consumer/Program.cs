using Amazon.SQS;
using Customers.Consumer;
using Customers.Consumer.Settings;
using MediatR;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<QueueSettings>(hostContext.Configuration.GetSection(QueueSettings.Key));
        services.AddMediatR(typeof(Program));
        services.AddSingleton<IAmazonSQS, AmazonSQSClient>();

        services.AddHostedService<MessageConsumerService>();
    })
    .Build();

host.Run();
