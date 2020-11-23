using Autofac;
using AzureFunctions.Autofac.Configuration;
using caching_example.Interfaces;
using caching_example.Services;

namespace caching_example.Config
{
    public class CachingConfig
    {
        public CachingConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterType<CacheTester>().As<ICacheTester>();
            }, functionName);
        }
    }
}
