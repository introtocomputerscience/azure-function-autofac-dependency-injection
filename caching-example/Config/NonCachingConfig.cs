using Autofac;
using AzureFunctions.Autofac.Configuration;
using caching_example.Interfaces;
using caching_example.Services;

namespace caching_example.Config
{
    public class NonCachingConfig
    {
        public NonCachingConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterType<CacheTester>().As<ICacheTester>().SingleInstance();
            }, functionName, enableCaching: false);
        }
    }
}
