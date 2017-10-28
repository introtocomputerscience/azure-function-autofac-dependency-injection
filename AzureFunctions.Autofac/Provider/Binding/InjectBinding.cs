using Autofac;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Autofac
{
    internal class InjectBinding : IBinding
    {
        private IInjectResolver resolver;
        private Type type;
        public bool FromAttribute => true;
        public InjectBinding(IInjectResolver resolver, Type type)
        {
            this.resolver = resolver;
            this.type = type;
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) =>
            Task.FromResult((IValueProvider)new InjectValueProvider(value));

        public async Task<IValueProvider> BindAsync(BindingContext context)
        {
            await Task.Yield();
            dynamic value = resolver.Resolve(type);
            return await BindAsync(value, context.ValueContext);
        }

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();
    }
}