using System;
using Tests.Integration.Setup;
using Xunit;

namespace Tests.Integration.Scenarios
{
    [Collection("DynamoDB")]
    public class TestAccountMetadata
    {
        private readonly TestContext _sut;

        public TestAccountMetadata(TestContext sut) => _sut = sut;
        
        [Fact]
        public void Test1()
        {
            var s = "s";
            Assert.Equal("s", s);
        }
    }
}