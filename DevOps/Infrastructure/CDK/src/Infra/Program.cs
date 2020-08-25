using Amazon.CDK;
using Amazon.CDK.AWS.ECS;

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
            var ecsCluster = new Cluster(mainStack, "app-cluster", new ClusterProps
            {
                ClusterName = "app-cluster",
                ContainerInsights = true
            });
            _ = app.CreateApiStack("SandBank");
            app.Synth();
        }
    }
}
