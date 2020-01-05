using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.S3;
using Amazon.S3.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SandBank.Lambda.ConfigAuditTrail
{
    public class Function
    {
        private IAmazonS3 _s3Client { get; set; }
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public Function()
        {
            _s3Client = new AmazonS3Client();
        }

        public Function(IAmazonS3 s3Client) => _s3Client = s3Client; //unit tests
        
        public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            context.Logger.LogLine($"RequestId: {context.AwsRequestId}, Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                context.Logger.LogLine($"RequestId: {context.AwsRequestId}, Event ID: {record.EventID}");

                var userId = record.Dynamodb.Keys["UserId"].N;
                var accountId = record.Dynamodb.Keys["AccountId"].N;
                var fileKey = $"user/{userId}/account/{accountId}.json";
                
                context.Logger.LogLine($"RequestId: {context.AwsRequestId}, userId: {userId}");
                context.Logger.LogLine($"RequestId: {context.AwsRequestId}, accountId: {accountId}");
                
                string streamRecordJson = Document.FromAttributeMap(record.Dynamodb.NewImage).ToJson();
                
                context.Logger.LogLine($"RequestId: {context.AwsRequestId}, DynamoDB Record: {streamRecordJson}");

                await _s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = "sandbank-config-audit-trail",
                    Key = fileKey,
                    ContentBody = streamRecordJson,
                });
                
                context.Logger.LogLine($"RequestId: {context.AwsRequestId}, Uploaded to S3 @ {fileKey}");
            }

            context.Logger.LogLine($"RequestId: {context.AwsRequestId}, Stream processing complete.");
        }
    }
}
