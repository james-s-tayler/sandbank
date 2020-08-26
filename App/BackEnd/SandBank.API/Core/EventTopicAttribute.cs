using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class EventTopicAttribute : Attribute
    {
        private readonly string _topicDisplayName;
        private readonly string _topicEndpoint;
        
        public string DisplayName => _topicDisplayName;
        public string TopicEndpoint => _topicEndpoint;

        public EventTopicAttribute(string topicDisplayName)
        {
            if (topicDisplayName == null || topicDisplayName.Equals(""))
            {
                throw new ArgumentException("You must specify the DisplayName for an EventTopic defined in appSettings.json");
            }

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                //.AddJsonFile($"appsettings.{environment}.json", false)
                .AddEnvironmentVariables()
                .Build();
            
            _topicDisplayName = topicDisplayName;
            _topicEndpoint = config[$"EventTopic:{_topicDisplayName}"];
            
            if (_topicEndpoint == null || _topicEndpoint.Equals(""))
            {
                //throw new ArgumentException($"You must specify the value for EventTopic:{_topicDisplayName} defined in appSettings.{environment}.json");
            }
        }
    }
}