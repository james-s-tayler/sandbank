using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;

namespace Infra
{
    public class ApiProps : StackProps
    {
        public IVpc Vpc { get; set; }
        public string ServiceName { get; set; }
        public string HostedZoneName { get; set; }
        public string HostedZoneId { get; set; }
        public Repository EcrRepository { get; set; }
        public Cluster EcsCluster { get; set; }
        public IDictionary<string, string> ContainerEnvVars { get; set; }
        public IDictionary<string, Secret> ContainerSecrets { get; set; }
        public ICertificate Certificate { get; set; }
        public string SubDomain { get; set; }
    }
}