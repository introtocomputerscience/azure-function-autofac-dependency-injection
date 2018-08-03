using AzureFunctions.Autofac;
using DurableFunctionsNetFrameworkExample.Configs;
using DurableFunctionsNetFrameworkExample.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DurableFunctionsNetFrameworkExample.Functions
{
    [DependencyInjectionConfig(typeof(AutofacConfig))]
    public static class ExampleFunction
    {

        [FunctionName("GreeterFunction")]
        public static async Task<string> Run([OrchestrationTrigger] DurableOrchestrationContextBase context, ILogger log, [Inject]IGreeter greeter)
        {
            var name = context.GetInput<string>();
            var greeting = greeter.Greet(name);
            var primaryGoodbye = await context.CallActivityAsync<string>("PrimaryGoodbye", name);
            var secondaryGoodbye = await context.CallActivityAsync<string>("SecondaryGoodbye", name);
            return $"{greeting} {primaryGoodbye} or {secondaryGoodbye}";
        }

        [FunctionName("PrimaryGoodbye")]
        public static async Task<string> PrimaryGoodbye([ActivityTrigger]string name, [Inject("Primary")]IGoodbyer goodbyer)
        {
            return goodbyer.Goodbye(name);
        }

        [FunctionName("SecondaryGoodbye")]
        public static async Task<string> SecondaryGoodbye([ActivityTrigger]string name, [Inject("Secondary")]IGoodbyer goodbyer)
        {
            return goodbyer.Goodbye(name);
        }

    }
}
