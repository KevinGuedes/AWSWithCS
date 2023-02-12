using Customers.Publisher.Messages;
using Customers.Publisher.Services;

namespace Customers.Publisher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISqsMessenger _sqsMessenger;

        public Worker(ILogger<Worker> logger, ISqsMessenger sqsMessenger)
        {
            _logger = logger;
            _sqsMessenger = sqsMessenger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var customerCreated = new CustomerCreated
            {
                DateOfBirth = DateTime.UtcNow,
                Email = "fake@gmail.com",
                FullName = "Fake User",
                GitHubUsername = "fakeuser",
                Id = Guid.NewGuid(),
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                await _sqsMessenger.SendMessageAsync(customerCreated, stoppingToken);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}