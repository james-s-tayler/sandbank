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
                GitHubSourceProps = new GitHubSourceProps    //move to paramater store
                {
                    Owner = "nicostouch",
                    Repo = "sandbank"
                },
                BuildSpecFile = "App/BackEnd/SandBank.API/Endpoints/buildspec.yml"
            });
            app.Synth();
        }
    }
}
