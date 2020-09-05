using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;

namespace Infra
{
    public static class ApiStackFactory
    {
        public static ApiStack CreateApiStack(this App app, 
            string apiName, 
            Cluster cluster, 
            Vpc vpc,
            Repository ecrRepo,
            Dictionary<string, string> containerEnvVars = null,
            Dictionary<string, Secret> containerSecrets = null,
            Environment env = null)
        {
            env ??= Constants.DefaultEnv; 
            containerEnvVars ??= new Dictionary<string, string>();
            containerSecrets ??= new Dictionary<string, Secret>();
            
            containerEnvVars.Add("AWS__REGION", env.Region);
            var serviceName = $"{apiName.ToLowerInvariant()}-api";
            
            return new ApiStack(app, $"{serviceName}-stack", new ApiProps
            {
                Vpc = vpc,
                Env = env,
                ServiceName = serviceName,
                EcsCluster = cluster,
                EcrRepository = ecrRepo,
                ContainerEnvVars = containerEnvVars,
                ContainerSecrets = containerSecrets
            });
        }
    }
}