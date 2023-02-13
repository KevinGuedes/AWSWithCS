using Amazon.SimpleNotificationService.Model;
using Shared.Customers.Messages;

namespace Customers.SNSPublisher.Services;

public interface ISnsMessenger
{
    Task<PublishResponse> PublishMessageAsync<T>(T message, CancellationToken cancellationToken) where T : IMessage;
}
