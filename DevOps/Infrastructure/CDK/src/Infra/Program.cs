using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.RDS;
using Secret = Amazon.CDK.AWS.ECS.Secret;

namespace Infra
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var hostedZoneName = "devcloudtest.com";
            var hostedZoneId = "Z0571072UHLSEXX7V3MP";
            
            var app = new App();
            var mainStack = new Stack(app, "main-stack", new StackProps
            {
                Env = Constants.DefaultEnv
            });
            
            var credentials = new GitHubSourceCredentials(mainStack, "github-source-credentials", new GitHubSourceCredentialsProps
            {
                AccessToken = SecretValue.SecretsManager("github/oauth/token")
            });
            
            var vpc = new Vpc(mainStack, "main-vpc", new VpcProps
            {
                Cidr = "10.0.0.0/16"
            });

            var db = new PostgresStack(app, "postgres-db-stack", new DatabaseInstanceProps
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
            }, new StackProps
            {
                Env = Constants.DefaultEnv
            });
            
            var containerEnvVars = new Dictionary<string, string>
            {
                {"DB__ADDRESS", db.Instance.InstanceEndpoint.SocketAddress}
            };
            var containerSecrets = new Dictionary<string, Secret>
            {
                {"DatabaseConnection", Secret.FromSecretsManager(db.Instance.Secret)}
            };
            
            var accountMetadataTable = new Table(mainStack, "AccountMetadata", new TableProps
            {
                TableName = "AccountMetadata",
                PartitionKey = new Attribute
                {
                    Name = "UserId",
                    Type = AttributeType.NUMBER
                },
                SortKey = new Attribute
                {
                    Name = "AccountId",
                    Type = AttributeType.NUMBER
                },
                Stream = StreamViewType.NEW_IMAGE
            });
            
            var ecsCluster = new Cluster(mainStack, "app-cluster", new ClusterProps
            {
                Vpc = vpc,
                ClusterName = "app-cluster",
                ContainerInsights = true
            });
            
            var fargateSslCertArn = SecretValue.SecretsManager("fargateSslCertArn").ToString();
            var albCert = Certificate.FromCertificateArn(mainStack, "alb-cert", fargateSslCertArn);

            var sandbankBuildInfra = app.CreateApiBuildStack("SandBank", vpc);
            var sandbankApi = app.CreateApiStack("SandBank",
                ecsCluster,
                vpc,
                sandbankBuildInfra.EcrRepository,
                "sandbank-api",
                hostedZoneName,
                hostedZoneId,
                albCert,
                containerEnvVars,
                containerSecrets);

            accountMetadataTable.GrantFullAccess(sandbankApi.FargateService.TaskDefinition.TaskRole);
            
            var cloudfrontCertArn = SecretValue.SecretsManager("cloudfrontcertarn").ToString();
            var cert = Certificate.FromCertificateArn(mainStack, "cloudfront-cert", cloudfrontCertArn);
            
            var sandbankSpa = new SpaStack(app, "sandbank-spa-stack", new SpaStackProps
            {
                Env = Constants.DefaultEnv,
                Vpc = vpc,
                ServiceName = "sandbank-spa",
                SubDomain = "sandbank",
                HostedZoneName = hostedZoneName,
                HostedZoneId = hostedZoneId,
                CloudFrontCert = cert,
                GitHubSourceProps = Constants.GithubRepo,
                BuildSpecFile = Constants.NpmBuildSpec,
                SpaDirectory = "App/FrontEnd/sandbank.spa",
                ApiUrl = $"{sandbankApi.ApiUrl}/api"
            });
            
            //lambda
            //SandBank.Lambda.ConfigAuditTrail::SandBank.Lambda.ConfigAuditTrail.Function::FunctionHandler
            
            app.Synth();
        }
    }
}