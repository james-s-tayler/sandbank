using Amazon;
using Amazon.SimpleNotificationService;
using MuchNeededAttributes;

namespace Integration.AWS.SNS
{
    [TechnicalDebt("This should be fully externally configurable.")]
    public class LocalstackSNSClientFactory : ISNSClientFactory
    {
        public string ServiceURL => $"{ProxyScheme}://{ProxyHostName}:{ProxyPort}/";
        
        [HardcodedValue("http")]
        public string ProxyScheme { get; set; } = "http";
        
        [HardcodedValue("localstack")]
        public string ProxyHostName { get; set; } = "localstack";
        
        [HardcodedValue("4575")]
        public int ProxyPort { get; set; } = 4575;

        [HardcodedValue("RegionEndpoint.USEast1")]
        public AmazonSimpleNotificationServiceClient CreateClient()
        {
            return new AmazonSimpleNotificationServiceClient(new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = ServiceURL,
                ProxyHost = ProxyHostName,
                ProxyPort = ProxyPort,
                UseHttp = ProxyScheme == "http",
                RegionEndpoint = RegionEndpoint.USEast1
            });
        }
    }
}