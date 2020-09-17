using Amazon.CDK;
using Amazon.CDK.AWS.EC2;

namespace Infra
{
    public static class ApiBuildStackFactory
    {
        public static ApiBuildStack CreateApiBuildStack(this Construct scope, 
            string apiName, 
            Vpc vpc, 
            Environment env = null)
        {
            env ??= Constants.DefaultEnv; 
            
            var serviceName = $"{apiName.ToLowerInvariant()}-api";
            
            return new ApiBuildStack(scope, $"{serviceName}-build-stack", new ApiBuildProps
            {
                Vpc = vpc,
                Env = env,
                ServiceName = serviceName,
                GitHubSourceProps = Constants.GithubRepo,
                BuildSpecFile = Constants.DockerBuildSpec,
                DockerfileLocation = Constants.Dockerfile,
                DockerContext = $"App/BackEnd/{apiName}.API/"
            });
        }
    }
}