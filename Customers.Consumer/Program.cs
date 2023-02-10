using Customers.Consumer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<CustomersConsumer>();
    })
    .Build();

host.Run();
