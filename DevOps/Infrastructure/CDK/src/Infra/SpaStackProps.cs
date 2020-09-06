using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.EC2;

namespace Infra
{
    public class SpaStackProps : StackProps
    {
        public IVpc Vpc { get; set; }
        public string ServiceName { get; set; }
        public GitHubSourceProps GitHubSourceProps { get; set; }
        public string BuildSpecFile { get; set; }
        public string SpaDirectory { get; set; }
    }
}