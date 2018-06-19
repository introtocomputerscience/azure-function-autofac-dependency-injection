using Autofac;
using AutofacDIExample.Modules;
using AzureFunctions.Autofac.Configuration;

namespace AutofacDIExample.Resolvers
{
    public class SecondaryConfig
    {
        public SecondaryConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterModule(new SecondaryTestModule());
            }, functionName);
        }
    }
}
