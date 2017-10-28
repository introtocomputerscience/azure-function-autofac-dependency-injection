using AutofacDIExample.DependencyInjection;
using AutofacDIExample.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;

namespace AutofacDIExample.GreeterFunction
{
    public static class GreeterFunction
    {
        [FunctionName("GreeterFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request, TraceWriter log, [Inject]IGreeter greeter)
        {
            log.Info("C# HTTP trigger function processed a request.");
            return request.CreateResponse(HttpStatusCode.OK, greeter.Greet());
        }
    }
}
