using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.ECR;

namespace Infra
{
    public class ApiBuildStack : Stack
    {
        public Repository EcrRepository { get; }

        public ApiBuildStack(Construct scope, string id, ApiBuildProps props = null) : base(scope, id,
            props)
        {
            EcrRepository = new Repository(this, $"{props.ServiceName}-repo", new RepositoryProps
            {
                RemovalPolicy = RemovalPolicy.DESTROY,
                RepositoryName = props.ServiceName,
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
                    {"IMAGE_REPO_NAME", new BuildEnvironmentVariable { Value = EcrRepository.RepositoryName }},
                    {"CONTEXT_PATH", new BuildEnvironmentVariable { Value = props.DockerContext }},
                    {"DOCKERFILE_PATH", new BuildEnvironmentVariable { Value = props.DockerfileLocation }},
                    {"CUSTOM_TAG", new BuildEnvironmentVariable { Value = "" }}
                }
                //example buildSpec here
                //https://blog.petrabarus.net/2020/03/23/building-ci-cd-pipeline-using-aws-codepipeline-aws-codebuild-amazon-ecr-amazon-ecs-with-aws-cdk/
            });
            
            EcrRepository.GrantPullPush(codeBuildProject);
            
            // useful https://cloudonaut.io/ecs-deployment-options/
        }
    }
}