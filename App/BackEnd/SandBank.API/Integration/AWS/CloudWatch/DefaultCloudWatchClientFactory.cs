using Amazon;
using Amazon.CloudWatch;

namespace Integration.AWS.CloudWatch
{
    public class DefaultCloudWatchClientFactory : ICloudWatchClientFactory
    {
        public IAmazonCloudWatch CreateClient()
        {
            return new AmazonCloudWatchClient(RegionEndpoint.USEast1);
        }
    }
}