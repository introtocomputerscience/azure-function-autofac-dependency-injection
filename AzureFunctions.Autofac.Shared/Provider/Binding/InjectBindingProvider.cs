#if !NET46
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
#endif
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

            var container = GetContainer(method.DeclaringType, attribute);

            //Check if there is a name property
            var injectAttribute = context.Parameter.GetCustomAttribute<InjectAttribute>();

            //This resolves the binding
            IBinding binding =
                new InjectBinding(container, context.Parameter.ParameterType, injectAttribute.Name);

            return Task.FromResult(binding);
        }

        private IContainer GetContainer(Type functionType, DependencyInjectionConfigAttribute attribute)
        {
            // Determine whether the container has already been created for the owning function class
            // We need a lock around this just in case this ends up being resolved in parallel for the parameters of a function method
            // Reads on the lock should far exceed writes so a ReaderWriterLockSlim is better than a lock statement
            syncLock.EnterUpgradeableReadLock();

            try
            {
                if (cache.ContainsKey(functionType))
                {
                    return cache[functionType];
                }

                syncLock.EnterWriteLock();

                try
                {
                    if (cache.ContainsKey(functionType))
                    {
                        return cache[functionType];
                    }

                    var container = BuildContainer(attribute);

                    cache[functionType] = container;

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

        private IContainer BuildContainer(DependencyInjectionConfigAttribute attribute)
        {
            var builder = new ContainerBuilder();

#if !NET46
            if (services != null)
            {
                builder.Populate(services);
            }
#endif

            // Use this next section to support the user configuration with a constructor that takes ContainerBuilder
            // _________________________________________________________________________________________________

            //var builderConstructor = attribute.Config.GetConstructor(new[] {typeof(ContainerBuilder)});

            //if (builderConstructor == null)
            //{
            //    throw new MissingMemberException(
            //        $"The {attribute.Config.FullName} class must provide a constructor that has a single parameter of type {typeof(ContainerBuilder).FullName}.");
            //}

            //Activator.CreateInstance(attribute.Config, builder);
            // _________________________________________________________________________________________________

            if (attribute.Config.IsAssignableTo<Module>() == false)
            {
                throw new InvalidOperationException(
                    $"The {attribute.Config.FullName} class must inherit from {typeof(Module).FullName} and provide a default constructor.");
            }

            var userModule = (Module)Activator.CreateInstance(attribute.Config);

            builder.RegisterModule(userModule);

            return builder.Build();
        }
    }
}