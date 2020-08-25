using Amazon.CDK;

namespace Pipeline
{
    public static class ApiStackFactory
    {
        public static ApiStack CreateApiStack(this App app, string apiName, Environment env = null)
        {
            var serviceName = $"{apiName.ToLowerInvariant()}-api";
            return new ApiStack(app, $"{serviceName}-stack", new ApiProps
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