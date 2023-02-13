using Amazon.SQS;
using Customers.SQSPublisher;
using Customers.SQSPublisher.Services;
using Customers.SQSPublisher.Settings;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<QueueSettings>(hostContext.Configuration.GetSection(QueueSettings.Key));

        services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
        services.AddSingleton<ISqsMessenger, SqsMessenger>();

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
