using Amazon.DynamoDBv2.DataModel;

namespace Models.DynamoDB
{
    [DynamoDBTable("SandBank_Account_Metadata")]
    public class AccountMetadata
    {
        [DynamoDBHashKey]
        public int UserId { get; set; }
        [DynamoDBRangeKey]
        public int AccountId { get; set; }
        public string Nickname { get; set; }
        public string ImageUrl { get; set; }
        public string LastModified { get; set; }
    }
}