using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;

namespace Pipeline
{
    public static class ApiStackFactory
    {
        public static ApiStack CreateApiStack(this App app, string apiName, Cluster cluster, Vpc vpc,
            Dictionary<string, string> containerEnvVars,
            Environment env = null)
        {
            env ??= Constants.DefaultEnv; 
            containerEnvVars ??= new Dictionary<string, string>();
            
            containerEnvVars.Add("AWS__REGION", env.Region);
            var serviceName = $"{apiName.ToLowerInvariant()}-api";
            
            return new ApiStack(app, $"{serviceName}-stack", cluster, new ApiProps
            {
                Vpc = vpc,
                Env = env,
                ServiceName = serviceName,
                GitHubSourceProps = Constants.githubRepo,
                BuildSpecFile = Constants.BuildSpec,
                DockerfileLocation = Constants.Dockerfile,
                DockerContext = $"App/BackEnd/{apiName}.API/",
                ContainerEnvVars = containerEnvVars
            });
        }
    }
}