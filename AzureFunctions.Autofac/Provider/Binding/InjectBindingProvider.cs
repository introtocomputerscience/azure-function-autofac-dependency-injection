using Autofac;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;

namespace AzureFunctions.Autofac
{
    public class InjectBindingProvider : IBindingProvider
    {
        public Task<IBinding> TryCreateAsync(BindingProviderContext context) {
            //Get the resolver
            MethodInfo method = context.Parameter.Member as MethodInfo;
            InjectResolverAttribute attribute = method.GetCustomAttribute<InjectResolverAttribute>();
            IInjectResolver resolver = (IInjectResolver)Activator.CreateInstance(attribute.Resolver);
            //This resolves the binding
            IBinding binding = new InjectBinding(resolver, context.Parameter.ParameterType);
            return Task.FromResult(binding);
        }
    }
}
