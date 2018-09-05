using Autofac;
using DurableFunctionsNetFrameworkExample.Interfaces;
using DurableFunctionsNetFrameworkExample.Models;

namespace DurableFunctionsNetFrameworkExample.Configs
{
    public class AutofacConfig : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Greeter>().As<IGreeter>();
            builder.RegisterType<Goodbyer>().Named<IGoodbyer>("Primary");
            builder.RegisterType<AlternateGoodbyer>().Named<IGoodbyer>("Secondary");
        }
    }
}
