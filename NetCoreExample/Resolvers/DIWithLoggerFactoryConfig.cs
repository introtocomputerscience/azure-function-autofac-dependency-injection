using Autofac;
using AutofacDIExample.Modules;
using AzureFunctions.Autofac.Configuration;
using AzureFunctions.Autofac.Shared.Extensions;
using Microsoft.Extensions.Logging;

namespace AutofacDIExample.Resolvers
{
    public class DIWithLoggerFactoryConfig
    {
        public DIWithLoggerFactoryConfig(string functionName, ILoggerFactory factory)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterModule(new TestModule());
                builder.RegisterLoggerFactory(factory);
            }, functionName);
        }
    }
}