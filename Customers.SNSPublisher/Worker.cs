using Customers.SNSPublisher.Messages;
using Customers.SNSPublisher.Services;

namespace Customers.SNSPublisher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISnsMessenger _snsMessenger;

        public Worker(ILogger<Worker> logger, ISnsMessenger snsMessenger)
        {
            _logger = logger;
            _snsMessenger = snsMessenger;
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
                await _snsMessenger.PublishMessageAsync(customerCreated, stoppingToken);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}