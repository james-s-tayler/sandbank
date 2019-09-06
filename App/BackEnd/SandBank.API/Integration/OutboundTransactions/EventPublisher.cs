using System.Threading.Tasks;
using Amazon.S3.Util;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;

namespace Integration.OutboundTransactions
{
    public class EventPublisher<T>
    {
        private readonly AmazonSimpleNotificationServiceClient _snsClient;
        private readonly string topic;
        
        public EventPublisher(string topic)
        {
            _snsClient = new AmazonSimpleNotificationServiceClient();
        }

        public async Task<PublishResponse> Publish(T entity)
        {
            return await _snsClient.PublishAsync(topic, JsonConvert.SerializeObject(entity));
        }
    }
}