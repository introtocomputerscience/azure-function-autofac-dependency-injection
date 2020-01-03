using AzureFunctions.Autofac.Exceptions;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Reflection;
using System.Threading.Tasks;
using AzureFunctions.Autofac.Configuration;

namespace AzureFunctions.Autofac
{
    public class InjectBindingProvider : IBindingProvider
    {
        private readonly string _appDirectory;

        public InjectBindingProvider()
        {
        }

        public InjectBindingProvider(string appDirectory)
        {
            _appDirectory = appDirectory;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context) {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            //Get the resolver starting with method then class
            MethodInfo method = context.Parameter.Member as MethodInfo;
            DependencyInjectionConfigAttribute attribute = method.DeclaringType.GetCustomAttribute<DependencyInjectionConfigAttribute>();
            if(attribute == null) { throw new MissingAttributeException(); }

            var functionName = method.DeclaringType.Name;
            
            //Initialize DependencyInjection
            var functionAndAppDirectoryConstructor = attribute.Config.GetConstructor(new[] { typeof(string), typeof(string) });

            if (functionAndAppDirectoryConstructor != null)
            {
                Activator.CreateInstance(attribute.Config, functionName, _appDirectory);
            }
            else
            {
                Activator.CreateInstance(attribute.Config, functionName);
            }
            
            //Check if there is a name property
            InjectAttribute injectAttribute = context.Parameter.GetCustomAttribute<InjectAttribute>();
            //This resolves the binding
            IBinding binding = new InjectBinding(context.Parameter.ParameterType, injectAttribute.Name, functionName);
            return Task.FromResult(binding);
        }
    }
}
