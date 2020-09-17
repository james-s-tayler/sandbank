using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.RDS;

namespace Infra
{
    public class QaStage : Stage
    {
        public QaStage(Construct scope, string id, QaStageProps props) : base(scope, id, props)
        {
            var mainStack = new Stack(this, "main-stack", new StackProps
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
            
            //should change this to Aurora Serverless!!!
            //https://dev.to/cjjenkinson/how-to-create-an-aurora-serverless-rds-instance-on-aws-with-cdk-5bb0

            var db = new PostgresStack(this, "postgres-db-stack", new DatabaseInstanceProps
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

            var sandbankBuildInfra = this.CreateApiBuildStack("SandBank", vpc);
            var sandbankApi = this.CreateApiStack("SandBank",
                ecsCluster,
                vpc,
                sandbankBuildInfra.EcrRepository,
                "sandbank-api",
                props.HostedZoneName,
                props.HostedZoneId,
                albCert,
                containerEnvVars,
                containerSecrets);

            accountMetadataTable.GrantFullAccess(sandbankApi.FargateService.TaskDefinition.TaskRole);
            
            var cloudfrontCertArn = SecretValue.SecretsManager("cloudfrontcertarn").ToString();
            var cert = Certificate.FromCertificateArn(mainStack, "cloudfront-cert", cloudfrontCertArn);
            
            var sandbankSpa = new SpaStack(this, "sandbank-spa-stack", new SpaStackProps
            {
                Env = Constants.DefaultEnv,
                Vpc = vpc,
                ServiceName = "sandbank-spa",
                SubDomain = "sandbank",
                HostedZoneName = props.HostedZoneName,
                HostedZoneId = props.HostedZoneId,
                CloudFrontCert = cert,
                GitHubSourceProps = Constants.GithubRepo,
                BuildSpecFile = Constants.NpmBuildSpec,
                SpaDirectory = "App/FrontEnd/sandbank.spa",
                ApiUrl = $"{sandbankApi.ApiUrl}/api" //maybe should use CfnOutput instead
            });
            
            //lambda
            //SandBank.Lambda.ConfigAuditTrail::SandBank.Lambda.ConfigAuditTrail.Function::FunctionHandler
        }
    }
}