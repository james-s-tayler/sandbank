using Amazon;
using Amazon.CloudWatchLogs;

namespace Integration.AWS.CloudWatch
{
    public class LocalstackCloudWatchLogsClientFactory : ICloudWatchLogsClientFactory
    {
        public IAmazonCloudWatchLogs CreateClient()
        {
            var config = new AmazonCloudWatchLogsConfig();
            config.UseHttp = true;
            config.RegionEndpoint = RegionEndpoint.USEast1;
            config.ServiceURL = "http://localhost:4586"; 
            
            return new AmazonCloudWatchLogsClient(config);
        }
    }
}