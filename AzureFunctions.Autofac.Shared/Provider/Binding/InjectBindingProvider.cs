using AzureFunctions.Autofac.Exceptions;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Reflection;
using System.Threading.Tasks;
using AzureFunctions.Autofac.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Autofac
{
    public class InjectBindingProvider : IBindingProvider
    {
        private readonly string _appDirectory;
        private readonly ILoggerFactory _loggerFactory;

        public InjectBindingProvider()
        {
        }

        public InjectBindingProvider(string appDirectory, ILoggerFactory loggerFactory)
        {
            _appDirectory = appDirectory;
            _loggerFactory = loggerFactory;
        }

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

            var functionName = method.DeclaringType.Name;

            //Initialize DependencyInjection
            var functionAndAppDirectoryAndLoggerFactoryConstructor = attribute.Config.GetConstructor(new[] { typeof(string), typeof(string), typeof(ILoggerFactory) });
            var functionAndAppDirectoryConstructor = attribute.Config.GetConstructor(new[] { typeof(string), typeof(string) });

            if (functionAndAppDirectoryAndLoggerFactoryConstructor != null)
            {
                Activator.CreateInstance(attribute.Config, functionName, _appDirectory, _loggerFactory);
            }
            else if (functionAndAppDirectoryConstructor != null)
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
