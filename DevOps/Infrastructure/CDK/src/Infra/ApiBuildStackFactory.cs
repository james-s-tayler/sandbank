using Amazon.CDK;
using Amazon.CDK.AWS.EC2;

namespace Infra
{
    public static class ApiBuildStackFactory
    {
        internal static ApiBuildStack CreateApiBuildStack(this App app, 
            string apiName, 
            Vpc vpc, 
            Environment env = null)
        {
            env ??= Constants.DefaultEnv; 
            
            var serviceName = $"{apiName.ToLowerInvariant()}-api";
            
            return new ApiBuildStack(app, $"{serviceName}-build-stack", new ApiBuildProps
            {
                Vpc = vpc,
                Env = env,
                ServiceName = serviceName,
                GitHubSourceProps = Constants.githubRepo,
                BuildSpecFile = Constants.BuildSpec,
                DockerfileLocation = Constants.Dockerfile,
                DockerContext = $"App/BackEnd/{apiName}.API/"
            });
        }
    }
}