using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.EC2;

namespace Infra
{
    public class SpaStackProps : StackProps
    {
        public IVpc Vpc { get; set; }
        public string ServiceName { get; set; }
        public GitHubSourceProps GitHubSourceProps { get; set; }
        public string BuildSpecFile { get; set; }
        public string SpaDirectory { get; set; }
        public string DomainName { get; set; }
        public ICertificate CloudFrontCert { get; set; }
        
        public string ApiUrl { get; set; }
    }
}