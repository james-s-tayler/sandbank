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
            
            //need to set up VPC - db must be passed vpc
            
            var db = new DatabaseInstance(mainStack, "postgres-db", new DatabaseInstanceProps
            {
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
            
            var ecsCluster = new Cluster(mainStack, "app-cluster", new ClusterProps
            {
                ClusterName = "app-cluster",
                ContainerInsights = true
            });
            _ = app.CreateApiStack("SandBank", ecsCluster);
            app.Synth();
        }
    }
}
