using Amazon.CDK;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.Pipelines;

namespace Infra
{
    public class PipelineStack : Stack
    {
        public PipelineStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var sourceArtifact = new Artifact_();
            var cloudAssemblyArtifact = new Artifact_();
            
            var pipeline = new CdkPipeline(this, "CdkPipeline", new CdkPipelineProps
            {
                PipelineName = "sandbank-api-pipeline",
                CdkCliVersion = "1.63.0",
                SourceAction = new GitHubSourceAction(new GitHubSourceActionProps
                {
                    ActionName = "synth-cdk",
                    Owner = "nicostouch",
                    Repo = "sandbank",
                    Output = sourceArtifact,
                    OauthToken = SecretValue.SecretsManager("github/oauth/token")
                }),
                SynthAction =  new SimpleSynthAction(new SimpleSynthActionProps
                {
                    SynthCommand = "cdk synth",
                    Subdirectory = "DevOps/Infrastructure/CDK",
                    SourceArtifact = sourceArtifact,
                    CloudAssemblyArtifact = cloudAssemblyArtifact
                }),
                CloudAssemblyArtifact = cloudAssemblyArtifact
            });
        }
    }
}