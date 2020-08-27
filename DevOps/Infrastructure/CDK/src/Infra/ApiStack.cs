using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.RDS;

namespace Pipeline
{
    public class ApiStack : Stack
    {
        internal ApiStack(Construct scope, string id, Cluster cluster, ApiProps props = null) : base(scope, id, props)
        {
            var repo = new Repository(this, $"{props.ServiceName}-repo", new RepositoryProps
            {
                RemovalPolicy = RemovalPolicy.DESTROY,
                RepositoryName = props.ServiceName,
            });

            var githubCredentials =
                new GitHubSourceCredentials(this, "github-credentials", new GitHubSourceCredentialsProps
                {
                    AccessToken = SecretValue.SecretsManager("github/oauth/token")
                });
            
            var codeBuildProject = new Project(this, $"{props.ServiceName}-codeBuild-project", new ProjectProps
            {
                Vpc = props.Vpc,
                ProjectName = props.ServiceName,
                Environment = new BuildEnvironment
                {
                    BuildImage = LinuxBuildImage.STANDARD_4_0,
                    Privileged = true
                },
                Source = Source.GitHub(props.GitHubSourceProps),
                BuildSpec = BuildSpec.FromSourceFilename(props.BuildSpecFile),
                EnvironmentVariables = new Dictionary<string, IBuildEnvironmentVariable>
                {
                    {"AWS_ACCOUNT_ID", new BuildEnvironmentVariable { Value = props.Env.Account }},
                    {"AWS_DEFAULT_REGION", new BuildEnvironmentVariable { Value = props.Env.Region }},
                    {"IMAGE_REPO_NAME", new BuildEnvironmentVariable { Value = repo.RepositoryName }},
                    {"CONTEXT_PATH", new BuildEnvironmentVariable { Value = props.DockerContext }},
                    {"DOCKERFILE_PATH", new BuildEnvironmentVariable { Value = props.DockerfileLocation }},
                    {"CUSTOM_TAG", new BuildEnvironmentVariable { Value = "" }}
                }
                //example buildSpec here
                //https://blog.petrabarus.net/2020/03/23/building-ci-cd-pipeline-using-aws-codepipeline-aws-codebuild-amazon-ecr-amazon-ecs-with-aws-cdk/
            });
            
            repo.GrantPullPush(codeBuildProject);
            
            _ = new ApplicationLoadBalancedFargateService(this, $"{props.ServiceName}-fargate-service", new ApplicationLoadBalancedFargateServiceProps
            {
                ServiceName = props.ServiceName,
                Cluster = cluster,
                TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                {
                    ContainerName = props.ServiceName,
                    Image = ContainerImage.FromEcrRepository(repo),
                    Environment = props.ContainerEnvVars,
                    Secrets = props.ContainerSecrets,
                    EnableLogging = true
                }
            });
            
            //seems handy https://github.com/aws/aws-cdk/issues/8352
            //also handy https://chekkan.com/iam-policy-perm-for-public-load-balanced-ecs-fargate-on-cdk/
        }
    }
}
