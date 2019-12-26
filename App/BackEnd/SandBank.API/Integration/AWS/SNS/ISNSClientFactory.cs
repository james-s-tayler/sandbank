using Amazon.SimpleNotificationService;

namespace Integration.AWS.SNS
{
    public interface ISNSClientFactory
    {
        AmazonSimpleNotificationServiceClient CreateClient();
    }
}