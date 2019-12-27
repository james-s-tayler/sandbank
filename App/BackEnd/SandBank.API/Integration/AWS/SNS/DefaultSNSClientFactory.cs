using Amazon;
using Amazon.SimpleNotificationService;
using TechDebtTags;

namespace Integration.AWS.SNS
{
    public class DefaultSNSClientFactory : ISNSClientFactory
    {
        [TechnicalDebt("should pull RegionEndpoint from config - not set it here...")]
        [HardcodedValue("RegionEndpoint.USEast1")]
        public AmazonSimpleNotificationServiceClient CreateClient()
        {
            return new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast1);
        }
    }
}