using System.Reflection;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Integration.OutboundTransactions
{
    public class EventPublisher<T>
    {
        private readonly AmazonSimpleNotificationServiceClient _snsClient;
        private readonly string _topicArn;
        private static ILogger<EventPublisher<T>> _logger;
        
        public EventPublisher(ILogger<EventPublisher<T>> logger, AmazonSimpleNotificationServiceClient snsClient)
        {
            _logger = logger;
            _snsClient = snsClient;

            var entityType = typeof(T);
            _logger.LogInformation($"publishing event for entityType:{entityType.AssemblyQualifiedName}");
            
            var eventTopic = entityType.GetCustomAttribute<EventTopicAttribute>();
            
            _topicArn = eventTopic.TopicEndpoint;
            _logger.LogInformation($"publishing event to endpoint:{_topicArn}");
        }

        public async Task<PublishResponse> Publish(T entity)
        {
            return await _snsClient.PublishAsync(_topicArn, JsonConvert.SerializeObject(entity));
        }
    }
}