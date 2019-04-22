using AutofacDIExample.Resolvers;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NetCoreExample.Interfaces;
using System.Net;
using System.Net.Http;

namespace NetCoreExample.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class AnnouncerFunction
    {
        [FunctionName("AnnouncerFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage request,
            ILogger log, [Inject] IAnnouncer announcer)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"Base Directory is: {announcer.Announce()}")
            };
        }
    }
}
