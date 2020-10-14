using Autofac;
using Autofac.Diagnostics.DotGraph;
using AzureFunctions.Autofac.Configuration;
using DOTGraphTracerExample.Interfaces;
using DOTGraphTracerExample.Models;
using System;
using System.IO;

namespace DOTGraphTracerExample.Configs
{
    public class DIConfig
    {
        public DIConfig(string functionName)
        {
            var tracer = new DotDiagnosticTracer();
            tracer.OperationCompleted += (sender, args) =>
            {
                // Writing the DOT trace to a file will let you render
                // it to a graph with Graphviz later, but this is
                // NOT A GOOD COPY/PASTE EXAMPLE. You'll want to do
                // things in an async fashion with good error handling.
                var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.dot");
                using var file = new StreamWriter(path);
                file.WriteLine(args.TraceContent);
            };
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterType<Greeter>().As<IGreeter>();
            }, functionName, c => c.SubscribeToDiagnostics(tracer));
        }
    }
}
