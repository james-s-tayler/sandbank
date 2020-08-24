using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.ECR;

namespace Pipeline
{
    public class ApiStack : Stack
    {
        internal ApiStack(Construct scope, string id, ApiProps props = null) : base(scope, id, props)
        {
            var repo = new Repository(this, $"{props.ServiceName}-repo", new RepositoryProps
            {
                RemovalPolicy = RemovalPolicy.DESTROY,
                RepositoryName = $"{props.ServiceName}-repo",
            });

            var githubCredentials =
                new GitHubSourceCredentials(this, "github-credentials", new GitHubSourceCredentialsProps
                {
                    AccessToken = SecretValue.SecretsManager("github/oauth/token")
                });
            
            var codeBuildProject = new Project(this, $"{props.ServiceName}-codeBuild-project", new ProjectProps
            {
                ProjectName = $"{props.ServiceName}-codebuild-project",
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
        }
    }
}
