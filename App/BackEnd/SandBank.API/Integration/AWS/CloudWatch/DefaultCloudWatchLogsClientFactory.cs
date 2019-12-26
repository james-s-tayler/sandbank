using Amazon;
using Amazon.CloudWatchLogs;

namespace Integration.AWS.CloudWatch
{
    public class DefaultCloudWatchLogsClientFactory : ICloudWatchLogsClientFactory
    {
        public IAmazonCloudWatchLogs CreateClient()
        {
            return new AmazonCloudWatchLogsClient(RegionEndpoint.USEast1);
        }
    }
}