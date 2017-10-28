using Autofac;
using AutofacDIExample.Interfaces;
using AutofacDIExample.Models;

namespace AutofacDIExample.Modules
{
    public class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Greeter>().As<IGreeter>();
            builder.RegisterType<Goodbyer>().As<IGoodbyer>();
        }
    }
}
