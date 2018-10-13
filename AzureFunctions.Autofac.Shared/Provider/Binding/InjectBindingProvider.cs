using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Autofac;

namespace AzureFunctions.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using AzureFunctions.Autofac.Exceptions;
    using Microsoft.Azure.WebJobs.Host.Bindings;
    using Module = global::Autofac.Module;

    public class InjectBindingProvider : IBindingProvider
    {
        private readonly IDictionary<Type, IContainer> cache = new Dictionary<Type, IContainer>();
        private readonly ReaderWriterLockSlim syncLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly IServiceCollection services;

        public InjectBindingProvider(IServiceCollection services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }

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
                throw new InitializationException(
                    $"The {attribute.Config.FullName} class must inherit from {typeof(Module).FullName} and provide a default constructor. See https://github.com/introtocomputerscience/azure-function-autofac-dependency-injection/blob/master/README.md for documentation and examples.");
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
            // Determine whether the container has already been created for the owning function class
            // We need a lock around this just in case this ends up being resolved in parallel for the parameters of a function method
            // Reads on the lock should far exceed writes so a ReaderWriterLockSlim is better than a lock statement
            syncLock.EnterUpgradeableReadLock();

            try
            {
                if (cache.ContainsKey(configModuleType))
                {
                    return cache[configModuleType];
                }

                syncLock.EnterWriteLock();

                try
                {
                    if (cache.ContainsKey(configModuleType))
                    {
                        return cache[configModuleType];
                    }

                    var container = BuildContainer(configModuleType);

                    cache[configModuleType] = container;

                    return container;
                }
                finally
                {
                    syncLock.ExitWriteLock();
                }
            }
            finally
            {
                syncLock.ExitUpgradeableReadLock();
            }
        }

        private IContainer BuildContainer(Type configModuleType)
        {
            var builder = new ContainerBuilder();
            
            if (services != null)
            {
                builder.Populate(services);
            }

            var userModule = (Module)Activator.CreateInstance(configModuleType);

            builder.RegisterModule(userModule);

            return builder.Build();
        }
    }
}