using AzureFunctions.Autofac;
using caching_example.Config;
using caching_example.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace caching_example
{
    [DependencyInjectionConfig(typeof(CachingConfig))]
    public class CacheExampleFunctions
    {
        [FunctionName("CachingEnabled")]
        public async Task Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log, [Inject]ICacheTester cacheTester)
        {
            var message = await cacheTester.GetMessage(DateTime.Now, 7000);
            log.LogInformation(message);
        }

        [FunctionName("CachingEnabledHTTP")]
        public async Task<IActionResult> Http(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICacheTester cacheTester)
        {
            var message = await cacheTester.GetMessage(DateTime.Now, 10000);
            log.LogInformation(message);
            return new OkObjectResult(message);
        }
    }
}
