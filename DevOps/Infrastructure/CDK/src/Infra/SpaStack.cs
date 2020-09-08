using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;

namespace Infra
{
    public class SpaStack : Stack
    {
        public SpaStack(Construct scope, string id, SpaStackProps props) : base(scope, id, props)
        {
            //s3 bucket
            var bucket = new Bucket(this, $"{props.ServiceName}-bucket", new BucketProps
            {
                WebsiteIndexDocument = "index.html",
                Versioned = true,
                BucketName = props.ServiceName,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            //cloudfront distribution
            var cloudFrontOai = new OriginAccessIdentity(this, $"{props.ServiceName}-oai", new OriginAccessIdentityProps
            {
                Comment = $"OAI for {props.ServiceName}."
            });

            var cloudfrontDist = new CloudFrontWebDistribution(this, $"{props.ServiceName}-cfd", new CloudFrontWebDistributionProps
            {
                ViewerCertificate = ViewerCertificate.FromAcmCertificate(
                    props.CloudFrontCert,
                    new ViewerCertificateOptions
                    {
                        Aliases = new []{ props.DomainName }, 
                        SslMethod = SSLMethod.SNI
                    }),
                OriginConfigs = new ISourceConfiguration[]
                {
                    new SourceConfiguration
                    {
                        S3OriginSource = new S3OriginConfig
                        {
                            S3BucketSource = bucket,
                            OriginAccessIdentity = cloudFrontOai
                        },
                        Behaviors = new IBehavior[]
                        {
                            new Behavior
                            {
                                IsDefaultBehavior = true,
                            }
                        }
                    }
                }
            });

            var cloudfrontS3Access = new PolicyStatement();
            cloudfrontS3Access.AddActions("s3:GetBucket*", "s3:GetObject*", "s3:List*");
            cloudfrontS3Access.AddResources(bucket.BucketArn);
            cloudfrontS3Access.AddResources($"{bucket.BucketArn}/*");
            cloudfrontS3Access.AddCanonicalUserPrincipal(cloudFrontOai.CloudFrontOriginAccessIdentityS3CanonicalUserId);

            bucket.AddToResourcePolicy(cloudfrontS3Access);
            
            //codebuild project

            var codeBuildProject = new Project(this, $"{props.ServiceName}-codeBuild-project", new ProjectProps
            {
                Vpc = props.Vpc,
                ProjectName = props.ServiceName,
                Environment = new BuildEnvironment
                {
                    BuildImage = LinuxBuildImage.STANDARD_4_0,
                },
                Source = Source.GitHub(props.GitHubSourceProps),
                BuildSpec = BuildSpec.FromSourceFilename(props.BuildSpecFile),
                EnvironmentVariables = new Dictionary<string, IBuildEnvironmentVariable>
                {
                    {"SPA_DIRECTORY", new BuildEnvironmentVariable { Value = props.SpaDirectory }},
                    {"S3_BUCKET", new BuildEnvironmentVariable { Value = bucket.BucketName }},
                    {"CLOUDFRONT_ID", new BuildEnvironmentVariable { Value = cloudfrontDist.DistributionId }},
                    {"API_URL", new BuildEnvironmentVariable { Value = props.ApiUrl }}
                }
            });
            
            // iam policy to push your build to S3
            codeBuildProject.AddToRolePolicy(
                new PolicyStatement(new PolicyStatementProps
                {
                    Effect = Effect.ALLOW,
                    Resources = new[] {bucket.BucketArn, $"{bucket.BucketArn}/*"},
                    Actions = new[]
                    {
                        "s3:GetBucket*",
                        "s3:List*",
                        "s3:GetObject*",
                        "s3:DeleteObject",
                        "s3:PutObject"
                    }
                }));
            
            codeBuildProject.AddToRolePolicy(
                new PolicyStatement(new PolicyStatementProps
                {
                    Effect = Effect.ALLOW,
                    Resources = new []{"*"},
                    Actions = new []
                    {
                        "cloudfront:CreateInvalidation",
                        "cloudfront:GetDistribution*",
                        "cloudfront:GetInvalidation",
                        "cloudfront:ListInvalidations",
                        "cloudfront:ListDistributions"
                    }
                }));
            
            //codepipeline?
        }
    }
}