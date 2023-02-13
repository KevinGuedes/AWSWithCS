using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.SQSPublisher.Settings;
using Microsoft.Extensions.Options;
using Shared.Customers.Messages;
using System.Text.Json;

namespace Customers.SQSPublisher.Services;

public class SqsMessenger : ISqsMessenger
{
    private readonly IAmazonSQS _sqs;
    private readonly QueueSettings _queueSettings;
    private string? _queueUrl;

    public SqsMessenger(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings)
    {
        _sqs = sqs;
        _queueSettings = queueSettings.Value;
    }

    public async Task<SendMessageResponse> SendMessageAsync<T>(
        T message,
        CancellationToken cancellationToken) where T : IMessage
    {
        var queueUrl = await GetQueueUrlAsync(cancellationToken);

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "MessageType", new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name
                    }
                }
            }
        };

        return await _sqs.SendMessageAsync(sendMessageRequest, cancellationToken);
    }

    private async ValueTask<string> GetQueueUrlAsync(CancellationToken cancellationToken)
    {
        if (_queueUrl is not null) return _queueUrl;

        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Name, cancellationToken);
        _queueUrl = queueUrlResponse.QueueUrl;
        return _queueUrl;
    }
}
