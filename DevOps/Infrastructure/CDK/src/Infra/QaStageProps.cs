using Amazon.CDK;

namespace Infra
{
    public class QaStageProps : StageProps
    {
        public string HostedZoneName { get; set; }
        public string HostedZoneId { get; set; }
    }
}