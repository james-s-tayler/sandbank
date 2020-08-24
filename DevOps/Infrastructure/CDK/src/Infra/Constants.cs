using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;

namespace Pipeline
{
    public class Constants
    {
        public const string Dockerfile = "DevOps/Infrastructure/Assets/generic-dotnet-Dockerfile";
        public const string BuildSpec = "DevOps/Infrastructure/Assets/generic-docker-buildspec.yml";

        public static Environment DefaultEnv = new Environment
        {
            Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
            Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
        };
        
        public static GitHubSourceProps githubRepo = new GitHubSourceProps
        {
            Owner = "nicostouch", //TODO: extract from current repository with
            Repo = "sandbank" //      git remote get-url --all origin
        };
    }
}