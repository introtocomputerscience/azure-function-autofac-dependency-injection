using AzureFunctions.Autofac.Provider.Config;
using AzureFunctions.Autofac.Startup;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(DependencyInjectionStartup))]

namespace AzureFunctions.Autofac.Startup
{
    public class DependencyInjectionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension(new InjectExtensionConfigProvider());
        }
    }
}
