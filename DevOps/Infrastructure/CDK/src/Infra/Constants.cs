using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;

namespace Infra
{
    public class Constants
    {
        private const string DevOpsAssets = "DevOps/Infrastructure/Assets";
        public static readonly string Dockerfile = $"{DevOpsAssets}/generic-dotnet-Dockerfile";
        public static readonly string DockerBuildSpec = $"{DevOpsAssets}/generic-docker-buildspec.yml";
        public static readonly string NpmBuildSpec = $"{DevOpsAssets}/generic-npm-buildspec.yml";

        public static readonly Environment DefaultEnv = new Environment
        {
            Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
            Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
        };
        
        public static readonly GitHubSourceProps GithubRepo = new GitHubSourceProps
        {
            Owner = "nicostouch",               //TODO: extract from current repository with
            Repo = "sandbank",                  //      git remote get-url --all origin
            WebhookFilters = new FilterGroup[]
            {
                FilterGroup.InEventOf(EventAction.PUSH, EventAction.PULL_REQUEST_MERGED).AndHeadRefIs("master")
            }
        };
    }
}