using Amazon.CDK;
using Amazon.CDK.AWS.ECS;

namespace Pipeline
{
    public static class ApiStackFactory
    {
        public static ApiStack CreateApiStack(this App app, string apiName, Cluster cluster, Environment env = null)
        {
            var serviceName = $"{apiName.ToLowerInvariant()}-api";
            return new ApiStack(app, $"{serviceName}-stack", cluster, new ApiProps
            {
                Env = env ?? Constants.DefaultEnv,
                ServiceName = serviceName,
                GitHubSourceProps = Constants.githubRepo,
                BuildSpecFile = Constants.BuildSpec,
                DockerfileLocation = Constants.Dockerfile,
                DockerContext = $"App/BackEnd/{apiName}.API/"
            });
        }
    }
}