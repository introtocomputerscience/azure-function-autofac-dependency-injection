using AzureFunctions.Autofac;
using caching_example.Config;
using caching_example.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace caching_example
{
    [DependencyInjectionConfig(typeof(CachingConfig))]
    public class CacheFunctions
    {
        [FunctionName("CachingEnabled")]
        public async Task Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log, [Inject]ICacheTester cacheTester)
        {
            var message = await cacheTester.GetMessage(DateTime.Now, 7000);
            log.LogInformation($"[Cache Enabled]{message}");
        }
    }

    [DependencyInjectionConfig(typeof(NonCachingConfig))]
    public class NoCacheFunctions
    {
        [FunctionName("CachingDisabled")]
        public async Task Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer, ILogger log, [Inject] ICacheTester cacheTester)
        {
            var message = await cacheTester.GetMessage(DateTime.Now, 7000);
            log.LogInformation($"[Cache Disabled]{message}");
        }
    }
}
