namespace Customers.Consumer
{
    public class CustomersConsumer : BackgroundService
    {
        private readonly ILogger<CustomersConsumer> _logger;

        public CustomersConsumer(ILogger<CustomersConsumer> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}