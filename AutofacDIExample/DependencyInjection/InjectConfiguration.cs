using Autofac;
using AutofacDIExample.Modules;
using Microsoft.Azure.WebJobs.Host.Config;

namespace AutofacDIExample.DependencyInjection
{
    public class InjectConfiguration : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new TestModule());
            var container = builder.Build();

            context.AddBindingRule<InjectAttribute>().Bind(new InjectBindingProvider(container));
        }
    }
}
