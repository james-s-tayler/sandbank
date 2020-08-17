using Amazon.CDK;

namespace Pipeline
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            _ = PipelineStack(app, "PipelineStack");
            app.Synth();
        }
    }
}
