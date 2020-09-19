using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;

namespace Infra
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            
            _ = new PipelineStack(app, "SandbankApiPipeline", new StackProps
            {
                Env = new Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION"),
                }
            });
            
            app.Synth();
        }
    }
}