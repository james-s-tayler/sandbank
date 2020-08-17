using Amazon.CDK;
using Amazon.CDK.AWS.ECR;

namespace Pipeline
{
    public class PipelineStack : Stack
    {
        internal PipelineStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var repo = new Repository(this, "sandbank-api-repo", new RepositoryProps
            {
                RemovalPolicy = RemovalPolicy.DESTROY,
                RepositoryName = "sandbank-api-repo",
            });
        }
    }
}
