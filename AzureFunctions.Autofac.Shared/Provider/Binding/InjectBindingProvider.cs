#if !NET46
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
#endif
using Autofac;

namespace AzureFunctions.Autofac
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Threading.Tasks;
    using AzureFunctions.Autofac.Exceptions;
    using Microsoft.Azure.WebJobs.Host.Bindings;
    using Module = global::Autofac.Module;

    public class InjectBindingProvider : IBindingProvider
    {
        private readonly ConcurrentDictionary<Type, IContainer> cache = new ConcurrentDictionary<Type, IContainer>();

        public InjectBindingProvider()
        {
        }

#if !NET46
        private readonly IServiceCollection services;

        public InjectBindingProvider(IServiceCollection services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }
#endif

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var method = (MethodInfo)context.Parameter.Member;
            var attribute = method.DeclaringType.GetCustomAttribute<DependencyInjectionConfigAttribute>();

            if (attribute == null)
            {
                throw new MissingAttributeException();
            }
            
            if (attribute.Config.IsAssignableTo<Module>() == false)
            {
                throw new InvalidOperationException(
                    $"The {attribute.Config.FullName} class must inherit from {typeof(Module).FullName} and provide a default constructor.");
            }

            var container = GetContainer(attribute.Config);

            //Check if there is a name property
            var injectAttribute = context.Parameter.GetCustomAttribute<InjectAttribute>();

            //This resolves the binding
            IBinding binding =
                new InjectBinding(container, context.Parameter.ParameterType, injectAttribute.Name);

            return Task.FromResult(binding);
        }

        private IContainer GetContainer(Type configModuleType)
        {
            return this.cache.GetOrAdd(configModuleType, BuildContainer(configModuleType));
        }

        private IContainer BuildContainer(Type configModuleType)
        {
            var builder = new ContainerBuilder();

#if !NET46
            if (services != null)
            {
                builder.Populate(services);
            }
#endif

            var userModule = (Module)Activator.CreateInstance(configModuleType);

            builder.RegisterModule(userModule);

            return builder.Build();
        }
    }
}