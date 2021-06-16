using Autofac;
using AzureFunctions.Autofac.Configuration;
using AzureFunctions.Autofac.Shared.Extensions;
using Microsoft.Extensions.Logging;

namespace FunctionsV3Example
{
    public class DIWithLoggerFactoryConfig
    {
        public DIWithLoggerFactoryConfig(string functionName, ILoggerFactory factory)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterType<LogWriter>().As<ILogWriter>();
                builder.RegisterLoggerFactory(factory);
            }, functionName);
        }
    }
}