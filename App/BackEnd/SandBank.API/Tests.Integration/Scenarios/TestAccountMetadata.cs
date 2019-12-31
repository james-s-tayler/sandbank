using System;
using System.Threading.Tasks;
using Core.Jwt;
using Microsoft.Extensions.DependencyInjection;
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
        public async Task Test1()
        {
            var s = "s";
            Assert.Equal("s", s);

            var jwtToken = _sut.ServiceProvider.GetService<IJwtTokenService>().GenerateToken(1);

            _sut.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
           var response = await _sut.Client.GetAsync($"api/user/test");
           
           Assert.Equal(200, (int)response.StatusCode);
           Assert.Equal("1", await response.Content.ReadAsStringAsync());
        }
    }
}