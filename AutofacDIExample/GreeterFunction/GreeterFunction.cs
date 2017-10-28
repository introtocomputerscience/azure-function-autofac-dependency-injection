using AutofacDIExample.Interfaces;
using AutofacDIExample.Resolvers;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;

namespace AutofacDIExample.GreeterFunction
{
    public class GreeterFunction
    {
        [InjectResolver(typeof(AutofacResolver))]
        [FunctionName("GreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, TraceWriter log, [Inject]IGreeter greeter, [Inject]IGoodbyer goodbye)
        {
            log.Info("C# HTTP trigger function processed a request.");
            return request.CreateResponse(HttpStatusCode.OK, $"{greeter.Greet()} {goodbye.Goodbye()}");
        }
    }
}
