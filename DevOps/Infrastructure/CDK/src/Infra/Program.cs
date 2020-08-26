using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
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
                AllocatedStorage = 1,
                BackupRetention = Duration.Days(0),
                DeletionProtection = false,
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE2, InstanceSize.MICRO),
                MasterUsername = "admin",
                MultiAz = false,
                DatabaseName = "postgres",
                RemovalPolicy = RemovalPolicy.DESTROY,
                AllowMajorVersionUpgrade = false
            });

            var rdsHostName = db.InstanceEndpoint.Hostname;
            var containerEnvironmentVars = new Dictionary<string, string>
            {
                {"DB_HOST", db.InstanceEndpoint.Hostname},
                {"DB_PORT",db.InstanceEndpoint.Port.ToString(CultureInfo.InvariantCulture)},
                {"DB_USER", "admin"}
            };
            
            var ecsCluster = new Cluster(mainStack, "app-cluster", new ClusterProps
            {
                Vpc = vpc,
                ClusterName = "app-cluster",
                ContainerInsights = true
            });
            
            _ = app.CreateApiStack("SandBank", ecsCluster, vpc, containerEnvironmentVars);
            
            app.Synth();
        }
    }
}