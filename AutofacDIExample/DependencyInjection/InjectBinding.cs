using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace AutofacDIExample.DependencyInjection
{
    internal class InjectBinding : IBinding
    {
        private IContainer container;
        private Type type;

        public InjectBinding(IContainer container, Type type)
        {
            this.container = container;
            this.type = type;
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) =>
            Task.FromResult((IValueProvider)new InjectValueProvider(value));

        public async Task<IValueProvider> BindAsync(BindingContext context)
        {
            await Task.Yield(); //Force Asynchronous Execution
            var value = container.Resolve(this.type);
            return await BindAsync(value, context.ValueContext);
        }

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();
    }
}