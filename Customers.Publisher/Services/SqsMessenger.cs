using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Messages;
using Customers.Publisher.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Customers.Publisher.Services;

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

    public async Task<SendMessageResponse> SendMessageAsync<T>(T message, CancellationToken cancellationToken) where T : IMessage
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

        return await _sqs.SendMessageAsync(sendMessageRequest);
    }

    private async Task<string> GetQueueUrlAsync(CancellationToken cancellationToken)
    {
        if (_queueUrl is not null) return _queueUrl;

        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Name, cancellationToken);
        _queueUrl = queueUrlResponse.QueueUrl;
        return _queueUrl;
    }
}
