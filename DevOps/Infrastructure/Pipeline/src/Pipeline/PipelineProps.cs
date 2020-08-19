using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;

namespace Pipeline
{
    public class PipelineProps : StackProps
    {
        public string ServiceName { get; set; }
        public GitHubSourceProps GitHubSourceProps { get; set; }
        public string BuildSpecFile { get; set; }
    }
}