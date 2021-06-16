using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FunctionsV3Example
{
    [DependencyInjectionConfig(typeof(DIWithLoggerFactoryConfig))]
    public static class LoggerFunction
    {
        [FunctionName("LoggerFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ILogWriter writer)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            writer.Log();
            return new OkObjectResult("Autofac managed to inject a ILogger<>");
        }
    }
}
