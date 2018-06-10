using Autofac;
using AzureFunctions.Autofac.Configuration;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Autofac
{
    internal class InjectBinding : IBinding
    {
        private Type type;
        private string name;

        public bool FromAttribute => true;
        public InjectBinding(Type type, String name)
        {
            this.type = type;
            this.name = name;
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) =>
            Task.FromResult((IValueProvider)new InjectValueProvider(value));

        public async Task<IValueProvider> BindAsync(BindingContext context)
        {
            await Task.Yield();
            dynamic value = DependencyInjection.Resolve(type, name);
            return await BindAsync(value, context.ValueContext);
        }

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();
    }
}