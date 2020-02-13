using System.Net;
using System.Net.Http;
using AutofacDIExample;
using AutofacDIExample.Resolvers;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace NetCoreExample.Functions
{
    [DependencyInjectionConfig(typeof(DIWithLoggerFactoryConfig))]
    public class LoggerFunction
    {
        [FunctionName("LoggerFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request,
            ILogger log,
            [Inject]ILogWriter writer)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            writer.Log();
            return request.CreateResponse(HttpStatusCode.OK, "Autofac managed to inject a ILogger<>");
        }
    }
}