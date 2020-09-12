using System.Security.Principal;
using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.Route53;
using HealthCheck = Amazon.CDK.AWS.ElasticLoadBalancingV2.HealthCheck;

namespace Infra
{
    public class ApiStack : Stack
    {
        public string ApiUrl { get; }
        public ApplicationLoadBalancedFargateService FargateService { get; }
        
        public ApiStack(Construct scope, string id, ApiProps props = null) : base(scope, id, props)
        {
            var hostedZone = HostedZone.FromHostedZoneAttributes(this, "HostedZone", new HostedZoneAttributes
            {
                ZoneName = props.HostedZoneName,
                HostedZoneId = props.HostedZoneId
            });
            
            FargateService = new ApplicationLoadBalancedFargateService(this, $"{props.ServiceName}-fargate-service", new ApplicationLoadBalancedFargateServiceProps
            {
                ServiceName = props.ServiceName,
                Cluster = props.EcsCluster,
                TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                {
                    ContainerName = props.ServiceName,
                    Image = ContainerImage.FromEcrRepository(props.EcrRepository),
                    Environment = props.ContainerEnvVars,
                    Secrets = props.ContainerSecrets,
                    EnableLogging = true
                },
                Certificate = props.Certificate,
                DomainName = $"{props.SubDomain}.{props.HostedZoneName}",
                DomainZone = hostedZone,
                //this has an internet-facing ALB open to the world - could enhance security by hiding behind an API gateway
            });

            FargateService.TargetGroup.ConfigureHealthCheck(new HealthCheck
            {
                Path = "/health"
            });

            ApiUrl = $"https://{props.SubDomain}.{props.HostedZoneName}";

            //seems handy https://github.com/aws/aws-cdk/issues/8352
            //also handy https://chekkan.com/iam-policy-perm-for-public-load-balanced-ecs-fargate-on-cdk/
        }
    }
}
