using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipeline
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new PipelineStack(app, "PipelineStack");
            app.Synth();
        }
    }
}
