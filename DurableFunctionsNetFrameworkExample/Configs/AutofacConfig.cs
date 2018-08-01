using Autofac;
using AzureFunctions.Autofac.Configuration;
using DurableFunctionsNetFrameworkExample.Interfaces;
using DurableFunctionsNetFrameworkExample.Models;

namespace DurableFunctionsNetFrameworkExample.Configs
{
    public class AutofacConfig
    {
        public AutofacConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterType<Greeter>().As<IGreeter>();
                builder.RegisterType<Goodbyer>().Named<IGoodbyer>("Primary");
                builder.RegisterType<AlternateGoodbyer>().Named<IGoodbyer>("Secondary");
            }, functionName);
        }
    }
}
