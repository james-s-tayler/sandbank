using Amazon.CloudWatchLogs;

namespace Integration.AWS.CloudWatch
{
    public interface ICloudWatchLogsClientFactory
    {
        IAmazonCloudWatchLogs CreateClient();
    }
}