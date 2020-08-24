using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;

namespace Pipeline
{
    public class ApiProps : StackProps
    {
        public string ServiceName { get; set; }
        public GitHubSourceProps GitHubSourceProps { get; set; }
        public string BuildSpecFile { get; set; }
        public string DockerfileLocation { get; set; }
        public string DockerContext { get; set; }
    }
}