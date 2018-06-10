using Autofac;
using Microsoft.Azure.WebJobs.Host.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureFunctions.Autofac.Attributes;
using AzureFunctions.Autofac.Provider.Binding;

namespace AzureFunctions.Autofac.Provider.Config
{
    public class InjectExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly InjectBindingProvider bindingProvider;
        public InjectExtensionConfigProvider()
        {
            bindingProvider = new InjectBindingProvider();
        }
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<InjectAttribute>().Bind(bindingProvider);
        }
    }
}

