using Amazon;
using Amazon.DynamoDBv2;

namespace Integration.AWS.DynamoDB
{
    public class DefaultDynamoDbClientFactory : IDynamoDbClientFactory
    {
        public IAmazonDynamoDB CreateClient()
        {
            return new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        }
    }
}