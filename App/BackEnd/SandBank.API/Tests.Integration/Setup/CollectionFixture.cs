using Xunit;

namespace Tests.Integration.Setup
{
    [CollectionDefinition("DynamoDB")]
    public class CollectionFixture : ICollectionFixture<TestContext> { }
}