using Amazon;
using Amazon.CloudWatch;

namespace Integration.AWS.CloudWatch
{
    public class LocalstackCloudWatchClientFactory : ICloudWatchClientFactory
    {
        public IAmazonCloudWatch CreateClient()
        {
            var config = new AmazonCloudWatchConfig();
            config.ProxyHost = "localstack";
            config.ProxyPort =  4582;
            config.UseHttp = true;
            config.RegionEndpoint = RegionEndpoint.USEast1;
            config.ServiceURL = "http://localhost:4582/";
            config.BufferSize = 0;

            return new AmazonCloudWatchClient(config);
        }
    }
}