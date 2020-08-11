using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Tests.Integration.Setup
{
    public class TestDataSetup
    {
        private static readonly IAmazonDynamoDB DynamoDBClient = new AmazonDynamoDBClient(
            new AmazonDynamoDBConfig { ServiceURL = "http://localhost:8000" });

        public static async Task CreateTable()
        {
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "UserId",
                        AttributeType = "N",
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "AccountId",
                        AttributeType = "N",
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "UserId",
                        KeyType = "HASH",
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "AccountId",
                        KeyType = "RANGE",
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 1,
                    WriteCapacityUnits = 1,
                },
                TableName = "SandBank_Account_Metadata",
            };

            await DynamoDBClient.CreateTableAsync(request);
            await WaitUntilTableActive(request.TableName);
        }

        private static async Task WaitUntilTableActive(string tableName)
        {
            string status = null;
            do
            {
                
                Thread.Sleep(1000);
                
                try
                {
                    status = await GetTableStatus(tableName);
                }
                catch 
                {
                    //swallow
                }
                
            } while (status != "ACTIVE");
        }

        private static async Task<string> GetTableStatus(string tableName)
        {
            var response = await DynamoDBClient.DescribeTableAsync(new DescribeTableRequest
            {
                TableName = tableName,
            });
            return response.Table.TableStatus;
        }
    }
}