using AzureFunctions.Autofac.Provider.Config;
using AzureFunctions.Autofac.Startup;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(DependencyInjectionStartup))]

namespace AzureFunctions.Autofac.Startup
{
    public class DependencyInjectionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension(new InjectExtensionConfigProvider());

            builder.Services.AddSingleton<IFunctionFilter, ScopeFilter>();
        }
    }
}
