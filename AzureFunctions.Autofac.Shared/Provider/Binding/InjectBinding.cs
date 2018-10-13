using Autofac;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Autofac
{
    internal class InjectBinding : IBinding
    {
        private readonly Type type;
        private readonly string name;
        private readonly IContainer container;

        public bool FromAttribute => true;

        public InjectBinding(IContainer container, Type type, string name)
        {
            this.container = container;
            this.type = type;
            this.name = name;
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) =>
            Task.FromResult((IValueProvider)new InjectValueProvider(value));

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            object value;
            
            if (string.IsNullOrWhiteSpace(name))
            {
                value = container.Resolve(type);
            }
            else
            {
                value = container.ResolveNamed(name, type);
            }

            // async/await not required here because there is no continuation
            // Simply returning the task result is better for perf because a state machine for the member is not compiled into the binary
            return BindAsync(value, context.ValueContext);
        }

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();
    }
}