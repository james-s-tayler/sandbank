using Amazon.DynamoDBv2;

namespace Integration.AWS.DynamoDB
{
    public interface IDynamoDbClientFactory
    {
        IAmazonDynamoDB CreateClient();
    }
}