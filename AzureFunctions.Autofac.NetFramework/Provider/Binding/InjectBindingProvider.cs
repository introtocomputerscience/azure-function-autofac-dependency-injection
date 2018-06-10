using AzureFunctions.Autofac.Exceptions;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureFunctions.Autofac
{
    public class InjectBindingProvider : IBindingProvider
    {
        public Task<IBinding> TryCreateAsync(BindingProviderContext context) {
            //Get the resolver starting with method then class
            MethodInfo method = context.Parameter.Member as MethodInfo;
            DependencyInjectionConfigAttribute attribute = method.DeclaringType.GetCustomAttribute<DependencyInjectionConfigAttribute>();
            if(attribute == null) { throw new MissingAttributeException(); } 
            //Initialize DependencyInjection
            Activator.CreateInstance(attribute.Config);
            //Check if there is a name property
            InjectAttribute injectAttribute = context.Parameter.GetCustomAttribute<InjectAttribute>();
            //This resolves the binding
            IBinding binding = new InjectBinding(context.Parameter.ParameterType, injectAttribute.Name);
            return Task.FromResult(binding);
        }
    }
}
