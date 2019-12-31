using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Core.Jwt;
using Entities.Domain.Accounts;
using Microsoft.Extensions.DependencyInjection;
using Models.DynamoDB;
using Newtonsoft.Json;
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
        public async Task GivenNoMetadata_WhenTryGetMetadata_NotFound()
        {
            var jwtToken = _sut.ServiceProvider.GetService<IJwtTokenService>().GenerateToken(1);

            _sut.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
           var response = await _sut.Client.GetAsync($"api/account/1/metadata");
           
           Assert.Equal(404, (int)response.StatusCode);
        }
        
        [Fact]
        public async Task GivenMetadata_WhenAddMetadata_Success()
        {
            var jwtToken = _sut.ServiceProvider.GetService<IJwtTokenService>().GenerateToken(1);

            var metadata = new AccountMetadata
            {
                Nickname = "Rainy Day Fund",
                ImageUrl = "http://localhost/lolcatz",
                AccountId = 1,
                UserId = 1,
                LastModified = "2020-01-01"
            };
            
            _sut.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
            _sut.Client.DefaultRequestHeaders.Accept.Clear();
            _sut.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var response = await _sut.Client.PostAsync($"api/account/1/metadata", 
                new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, "application/json"));
            Assert.Equal(200, (int)response.StatusCode);
            
            var response2 = await _sut.Client.GetAsync($"api/account/1/metadata");
            var metadataResponse =
                JsonConvert.DeserializeObject<AccountMetadataViewModel>(await response2.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)response.StatusCode);
            Assert.Equal("Rainy Day Fund", metadataResponse.Nickname);
        }
    }
}