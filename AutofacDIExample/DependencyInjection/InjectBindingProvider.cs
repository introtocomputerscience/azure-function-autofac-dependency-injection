using Autofac;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutofacDIExample.DependencyInjection
{
    public class InjectBindingProvider : IBindingProvider
    {
        private readonly IContainer container;

        public InjectBindingProvider(IContainer container) => this.container = container;
        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            IBinding binding = new InjectBinding(container, context.Parameter.ParameterType);
            return Task.FromResult(binding);
        }
    }
}
