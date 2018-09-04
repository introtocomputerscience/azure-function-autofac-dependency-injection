using AzureFunctions.Autofac.Exceptions;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Reflection;
using System.Threading.Tasks;
#if !NET46
using Microsoft.Extensions.DependencyInjection;
#endif

namespace AzureFunctions.Autofac
{
    public class InjectBindingProvider : IBindingProvider
    {
        public InjectBindingProvider()
        {
        }
#if !NET46
        private readonly IServiceCollection _services;

        public InjectBindingProvider(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }
#endif

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            //Get the resolver starting with method then class
            MethodInfo method = context.Parameter.Member as MethodInfo;
            DependencyInjectionConfigAttribute attribute = method.DeclaringType.GetCustomAttribute<DependencyInjectionConfigAttribute>();
            if (attribute == null) { throw new MissingAttributeException(); }

            var functionClassName = method.DeclaringType.Name;
            
            // TODO: Replace this constructor call with a wrapper around container construction
            // treating the attribute target type as an Autofac module 
            //Initialize DependencyInjection
            Activator.CreateInstance(attribute.Config, new Object[] { functionClassName });
            
            // At this point we should have created a container for the function if it hasn't already been created
            // This provider should have access to that container and provide it to the InjectBinding instance
            // Need to prove that this provider is not created more than once per instance of a function or call of a function
            // This means that we don't want it to be created too often
            
            //Check if there is a name property
            InjectAttribute injectAttribute = context.Parameter.GetCustomAttribute<InjectAttribute>();
            //This resolves the binding
            IBinding binding = new InjectBinding(context.Parameter.ParameterType, injectAttribute.Name, functionClassName);
            return Task.FromResult(binding);
        }
    }
}
