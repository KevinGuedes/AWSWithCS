using Amazon.SimpleNotificationService;
using Customers.SNSPublisher;
using Customers.SNSPublisher.Services;
using Customers.SNSPublisher.Settings;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<TopicSettings>(hostContext.Configuration.GetSection(TopicSettings.Key));
        services.AddSingleton<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>();
        services.AddSingleton<ISnsMessenger, SnsMessenger>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
