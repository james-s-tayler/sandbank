using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;

namespace Infra
{
    public class PostgresStack : Stack
    {
        public DatabaseInstance Instance { get; }
        
        public PostgresStack(Construct scope, string id, DatabaseInstanceProps props, StackProps stackProps) : base(scope, id, stackProps)
        {
            Instance = new DatabaseInstance(this, id, props);
            Instance.Connections.AllowFrom(Peer.Ipv4(props.Vpc.VpcCidrBlock), Port.Tcp(5432));
        }
    }
}