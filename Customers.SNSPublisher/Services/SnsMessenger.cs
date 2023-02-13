using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Customers.SNSPublisher.Settings;
using Microsoft.Extensions.Options;
using Shared.Customers.Messages;
using System.Text.Json;

namespace Customers.SNSPublisher.Services;

public class SnsMessenger : ISnsMessenger
{
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly TopicSettings _topicSettings;
    private string? _topicArn;

    public SnsMessenger(
        IAmazonSimpleNotificationService sns,
        IOptions<TopicSettings> topicSettings)
    {
        _sns = sns;
        _topicSettings = topicSettings.Value;
    }

    public async Task<PublishResponse> PublishMessageAsync<T>(T message, CancellationToken cancellationToken) where T : IMessage
    {
        var topicArn = await GetTopicArnAsync();

        var sendMessageRequest = new PublishRequest
        {
            TopicArn = topicArn,
            Message = JsonSerializer.Serialize(message),
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

        return await _sns.PublishAsync(sendMessageRequest, cancellationToken);
    }


    private async ValueTask<string> GetTopicArnAsync()
    {
        if (_topicArn is not null)
        {
            return _topicArn;
        }

        var queueUrlResponse = await _sns.FindTopicAsync(_topicSettings.Name);
        _topicArn = queueUrlResponse.TopicArn;
        return _topicArn;
    }
}
