using Autofac;
using AutofacDIExample.Modules;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;

namespace AutofacDIExample.Resolvers
{
    public class AutofacResolver : IInjectResolver
    {
        private readonly IContainer container;

        public AutofacResolver()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new TestModule());
            this.container = builder.Build();
        }

        public object Resolve(Type type)
        {
            return this.container.Resolve(type);
        }
    }
}
