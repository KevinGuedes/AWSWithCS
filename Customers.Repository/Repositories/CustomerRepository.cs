using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Customers.Repository.Contracts.Data;
using System.Net;
using System.Text.Json;

namespace Customers.Repository.Repositories;

internal class CustomerRepository
{
    private readonly AmazonDynamoDBClient _dynamoDb;
    private readonly string _tableName = "customers";

    public CustomerRepository()
        => (_dynamoDb) = (new AmazonDynamoDBClient());

    public async Task<bool> CreateAsync(CustomerDto customer, CancellationToken cancellationToken)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsAttributes = Document.FromJson(customerAsJson).ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = customerAsAttributes,
            ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)"
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<CustomerDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var getItemRequest = new GetItemRequest
        {
            ConsistentRead = false,
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.GetItemAsync(getItemRequest, cancellationToken);
        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }

    public async Task<CustomerDto?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var queryRequest = new QueryRequest
        {
            TableName = _tableName,
            IndexName = "email-id-index", //GSI
            KeyConditionExpression = "Email = :v_Email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {
                    ":v_Email", new AttributeValue{ S = email }
                }
            }
        };

        var response = await _dynamoDb.QueryAsync(queryRequest, cancellationToken);
        if (response.Items.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Items[0]);
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }

    //Very expensive approach, can be done like this but it is highly not recommended
    //public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    //{
    //    var scanRequest = new ScanRequest
    //    {
    //        TableName = _tableName
    //    };
    //    var response = await _dynamoDb.ScanAsync(scanRequest);
    //    return response.Items.Select(x =>
    //    {
    //        var json = Document.FromAttributeMap(x).ToJson();
    //        return JsonSerializer.Deserialize<CustomerDto>(json);
    //    })!;
    //}

    public async Task<bool> UpdateAsync(
        CustomerDto customer,
        DateTime requestStarted,
        CancellationToken cancellationToken)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsAttributes = Document.FromJson(customerAsJson).ToAttributeMap();

        var updateItemRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = customerAsAttributes,
            ConditionExpression = "UpdatedAt < :requestStarted",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":requestStarted", new AttributeValue{S = requestStarted.ToString("O")} }
            }
        };

        var response = await _dynamoDb.PutItemAsync(updateItemRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deletedItemRequest = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.DeleteItemAsync(deletedItemRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> CreateCustomersInBatchAsync(List<CustomerDto> customers, CancellationToken cancellationToken)
    {
        var transactItems = customers.Select(customer =>
        {
            customer.UpdatedAt = DateTime.UtcNow;
            var customerAsJson = JsonSerializer.Serialize(customer);
            var customerAsAttributeMap = Document.FromJson(customerAsJson).ToAttributeMap();

            return new TransactWriteItem
            {
                Put = new Put
                {
                    ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)",
                    TableName = _tableName, //can be a different table for every item
                    Item = customerAsAttributeMap
                }
            };
        });

        var transctRequest = new TransactWriteItemsRequest
        {
            TransactItems = transactItems.ToList()
        };

        var response = await _dynamoDb.TransactWriteItemsAsync(transctRequest, cancellationToken);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}

