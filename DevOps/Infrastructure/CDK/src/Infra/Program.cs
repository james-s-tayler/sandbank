using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.RDS;

namespace Pipeline
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            var mainStack = new Stack(app, "main-stack", new StackProps
            {
                Env = Constants.DefaultEnv
            });
            
            var vpc = new Vpc(mainStack, "main-vpc", new VpcProps
            {
                Cidr = "10.0.0.0/16"
            });
            
            var db = new DatabaseInstance(mainStack, "postgres-db", new DatabaseInstanceProps
            {
                Vpc = vpc,
                Engine = DatabaseInstanceEngine.Postgres(new PostgresInstanceEngineProps
                {
                    Version = PostgresEngineVersion.VER_12_3
                }),
                AllocatedStorage = 5,
                BackupRetention = Duration.Days(0),
                DeletionProtection = false,
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE2, InstanceSize.MICRO),
                MasterUsername = "sandbankadmin",
                MultiAz = false,
                DatabaseName = "postgres",
                RemovalPolicy = RemovalPolicy.DESTROY,
                AllowMajorVersionUpgrade = false
            });
            db.Connections.AllowFrom(Peer.Ipv4("10.0.0.0/16"), Port.Tcp(5432));

            var containerEnvVars = new Dictionary<string, string>
            {
                {"DB__ADDRESS", db.InstanceEndpoint.SocketAddress}
            };
            var containerSecrets = new Dictionary<string, Secret>
            {
                {"DatabaseConnection", Secret.FromSecretsManager(db.Secret)}
            };
            
            var ecsCluster = new Cluster(mainStack, "app-cluster", new ClusterProps
            {
                Vpc = vpc,
                ClusterName = "app-cluster",
                ContainerInsights = true
            });
            
            var sandbankApi = app.CreateApiStack("SandBank",
                ecsCluster, 
                vpc, 
                containerEnvVars,
                containerSecrets);
            
            
            app.Synth();
        }
    }
}