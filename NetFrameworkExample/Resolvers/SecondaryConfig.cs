using Autofac;
using AutofacDIExample.Modules;

namespace AutofacDIExample.Resolvers
{
    public class SecondaryConfig : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new SecondaryTestModule());
        }
    }
}