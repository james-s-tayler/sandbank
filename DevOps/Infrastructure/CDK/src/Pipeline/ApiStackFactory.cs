using Amazon.CDK;

namespace Pipeline
{
    public static class ApiStackFactory
    {
        public static ApiStack CreateApiStack(this App app, string apiName, Environment env = null)
        {
            return new ApiStack(app, $"{apiName}ApiStack", new ApiProps
            {
                Env = env ?? Constants.DefaultEnv,
                ServiceName = $"{apiName}Api",
                GitHubSourceProps = Constants.githubRepo,
                BuildSpecFile = Constants.BuildSpec,
                DockerfileLocation = Constants.Dockerfile,
                DockerContext = $"App/BackEnd/{apiName}.API/"
            });
        }
    }
}