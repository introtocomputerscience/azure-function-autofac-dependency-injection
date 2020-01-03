using AutofacDIExample.Resolvers;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;

namespace AutofacDIExample.GreeterFunction
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public class GreeterFunction
    {
        [FunctionName("GreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request,
                                              ILogger log,
                                              [Inject]IGreeter greeter,
                                              [Inject("Main")]IGoodbyer goodbye,
                                              [Inject("Secondary")]IGoodbyer alternateGoodbye)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"{greeter.Greet()} {goodbye.Goodbye()} or {alternateGoodbye.Goodbye()}")
            };
        }
    }
}
