using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Shared.Customers.Messages;
using System.Text.Json;

namespace Customers.Consumer;

internal class MessageConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly QueueSettings _queueSettings;
    private readonly IMediator _mediator;
    private readonly ILogger<MessageConsumerService> _logger;
    private string? _queueUrl;

    public MessageConsumerService(
        ILogger<MessageConsumerService> logger,
        IAmazonSQS sqs,
        IOptions<QueueSettings> queueSettings,
        IMediator mediator)
    {
        _sqs = sqs;
        _queueSettings = queueSettings.Value;
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = await GetQueueUrlAsync(stoppingToken);

        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            AttributeNames = new() { "All" },
            MessageAttributeNames = new() { "All" },
            MaxNumberOfMessages = 1
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

            foreach(var message in response.Messages)
            {
                var messageType = message.MessageAttributes["MessageType"].StringValue;
                var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");

                if (type is null)
                {
                    _logger.LogWarning("Unknown message type: {MessageType}", messageType);
                    continue;
                }

                var typedMessage = (IMessage)JsonSerializer.Deserialize(message.Body, type)!;

                try
                {
                    await _mediator.Send(typedMessage, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Message failed during processing");
                    continue;
                }

                await _sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle, stoppingToken);
            }
        }
    }

    private async ValueTask<string> GetQueueUrlAsync(CancellationToken cancellationToken)
    {
        if (_queueUrl is not null) return _queueUrl;

        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Name, cancellationToken);
        _queueUrl = queueUrlResponse.QueueUrl;
        return _queueUrl;
    }
}
