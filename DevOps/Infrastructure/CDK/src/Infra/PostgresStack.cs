using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.SecretsManager;

namespace Infra
{
    public class PostgresStack : Stack
    {
        public DatabaseInstance Instance { get; }
        
        public PostgresStack(Construct scope, string id, DatabaseInstanceProps props, StackProps stackProps) : base(scope, id, stackProps)
        {
            Instance = new DatabaseInstance(this, id, props);
            Instance.Connections.AllowFrom(Peer.Ipv4(props.Vpc.VpcCidrBlock), Port.Tcp(5432));
            
            //fix secret issues - invalid connection string can be generate without excluding these chars.
            var dbSecret = Instance.Node.TryFindChild("Secret") as DatabaseSecret;
            var cfnSecret = dbSecret.Node.DefaultChild as CfnSecret;
            cfnSecret.AddPropertyOverride("GenerateSecretString.ExcludeCharacters", "'\\;@$\"`!/");
        }
    }
}