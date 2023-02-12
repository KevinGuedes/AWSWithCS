using Customers.Consumer.Messages;
using FluentResults;
using MediatR;

namespace Customers.Consumer.Handlers;

internal class CustomerCreatedHandler : IRequestHandler<CustomerCreated, Result>
{
    private readonly ILogger<CustomerCreatedHandler> _logger;

    public CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task<Result> Handle(CustomerCreated request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(request.FullName);
        return Task.FromResult(Result.Ok());
    }
}
