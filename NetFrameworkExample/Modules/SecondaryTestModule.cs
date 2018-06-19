using Autofac;
using AutofacDIExample;
using AutofacDIExample.Models;

namespace AutofacDIExample.Modules
{
    public class SecondaryTestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Greeter>().As<IGreeter>();
            builder.RegisterType<Goodbyer>().Named<IGoodbyer>("Main");
            builder.RegisterType<SecondaryGoodbyer>().Named<IGoodbyer>("Secondary");
        }
    }
}
