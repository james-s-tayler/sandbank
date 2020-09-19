using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;

namespace Infra
{
    public static class ApiStackFactory
    {
        //this class is a pretty thin useless wrapper now that I look at it...
        public static ApiStack CreateApiStack(this Construct scope,
            string apiName,
            Cluster cluster,
            Vpc vpc,
            Repository ecrRepo,
            string subDomain,
            string hostedZoneName,
            string hostedZoneId,
            ICertificate sslCert,
            Dictionary<string, string> containerEnvVars = null,
            Dictionary<string, Secret> containerSecrets = null,
            Environment env = null)
        {
            env ??= Constants.DefaultEnv; 
            containerEnvVars ??= new Dictionary<string, string>();
            containerSecrets ??= new Dictionary<string, Secret>();
            
            containerEnvVars.Add("AWS__REGION", env.Region);
            var serviceName = $"{apiName.ToLowerInvariant()}-api";
            
            return new ApiStack(scope, $"{serviceName}-stack", new ApiProps
            {
                Vpc = vpc,
                Env = env,
                ServiceName = serviceName,
                EcsCluster = cluster,
                EcrRepository = ecrRepo,
                ContainerEnvVars = containerEnvVars,
                ContainerSecrets = containerSecrets,
                SubDomain = subDomain,
                HostedZoneName = hostedZoneName,
                HostedZoneId = hostedZoneId,
                Certificate = sslCert
            });
        }
    }
}