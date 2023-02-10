using Amazon.SQS.Model;
using Customers.Messages;

namespace Customers.Publisher.Services;

public interface ISqsMessenger
{
    Task<SendMessageResponse> SendMessageAsync<T>(
        T message,
        CancellationToken cancellationToken) where T : IMessage;
}
