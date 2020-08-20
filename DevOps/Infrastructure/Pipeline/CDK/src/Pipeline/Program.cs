using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;

namespace Pipeline
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            _ = new PipelineStack(app, "PipelineStack", new PipelineProps
            {
                Env = new Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
                },
                ServiceName = "sandbank-api",
                GitHubSourceProps = new GitHubSourceProps    //move to parameter store
                {
                    Owner = "nicostouch",
                    Repo = "sandbank"
                },
                BuildSpecFile = "DevOps/Infrastructure/Pipeline/generic-docker-buildspec.yml",
                DockerfileLocation = "DevOps/Infrastructure/Pipeline/generic-dotnet-Dockerfile",
                DockerContext = "App/BackEnd/SandBank.API/"
            });
            app.Synth();
        }
    }
}
