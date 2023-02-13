using Amazon.SQS.Model;
using Shared.Customers.Messages;

namespace Customers.SQSPublisher.Services;

public interface ISqsMessenger
{
    Task<SendMessageResponse> SendMessageAsync<T>(
        T message,
        CancellationToken cancellationToken) where T : IMessage;
}
