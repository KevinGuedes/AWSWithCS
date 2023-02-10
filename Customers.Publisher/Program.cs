using Amazon.SQS;
using Customers.Publisher;
using Customers.Publisher.Services;
using Customers.Publisher.Settings;

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
