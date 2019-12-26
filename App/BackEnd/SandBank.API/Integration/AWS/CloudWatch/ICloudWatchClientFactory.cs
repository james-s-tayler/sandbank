using Amazon.CloudWatch;

namespace Integration.AWS.CloudWatch
{
    public interface ICloudWatchClientFactory
    {
        IAmazonCloudWatch CreateClient();
    }
}