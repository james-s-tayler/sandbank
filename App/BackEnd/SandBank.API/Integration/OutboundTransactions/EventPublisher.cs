using System.Reflection;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Core;
using Newtonsoft.Json;

namespace Integration.OutboundTransactions
{
    public class EventPublisher<T>
    {
        private readonly AmazonSimpleNotificationServiceClient _snsClient;
        private readonly string _topicArn;
        
        public EventPublisher()
        {
            _snsClient = new AmazonSimpleNotificationServiceClient();

            var entityType = typeof(T);
            var eventTopic = entityType.GetCustomAttribute<EventTopicAttribute>();
            
            _topicArn = eventTopic.TopicEndpoint;
        }

        public async Task<PublishResponse> Publish(T entity)
        {
            return await _snsClient.PublishAsync(_topicArn, JsonConvert.SerializeObject(entity));
        }
    }
}