using Microsoft.Azure.WebJobs.Host.Config;
#if !NET46
using Microsoft.Extensions.DependencyInjection;
#endif
using System;

namespace AzureFunctions.Autofac.Provider.Config
{
    public class InjectExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly InjectBindingProvider bindingProvider;
        
#if !NET46
        public InjectExtensionConfigProvider(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            this.bindingProvider = new InjectBindingProvider(services);
        }
        
#endif
        
        public InjectExtensionConfigProvider()
        {
            this.bindingProvider = new InjectBindingProvider();
        }

        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<InjectAttribute>().Bind(this.bindingProvider);
        }
    }
}

