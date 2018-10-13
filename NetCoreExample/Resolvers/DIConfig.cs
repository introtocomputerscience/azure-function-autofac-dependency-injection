namespace AutofacDIExample.Resolvers
{
    using Autofac;
    using AutofacDIExample.Modules;

    public class DIConfig : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new TestModule());
        }
    }
}