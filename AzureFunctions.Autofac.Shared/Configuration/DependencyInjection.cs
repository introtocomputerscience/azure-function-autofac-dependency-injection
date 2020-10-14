using Autofac;
using AzureFunctions.Autofac.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace AzureFunctions.Autofac.Configuration
{
    public static class DependencyInjection
    {
        private static ConcurrentDictionary<string, IContainer> containers = new ConcurrentDictionary<string, IContainer>();
        private static ConcurrentDictionary<Guid, ILifetimeScope> instanceContainers = new ConcurrentDictionary<Guid, ILifetimeScope>();
        public static void Initialize(Action<ContainerBuilder> cfg, string functionClassName, Action<IContainer> containerAction = null)
        {
            ContainerBuilder builder = new ContainerBuilder();
            cfg(builder);
            var container = builder.Build();
            containerAction?.Invoke(container);
            containers.GetOrAdd(functionClassName, str => container);
        }

        public static object Resolve(Type type, string name, string functionClassName, Guid functionInstanceId)
        {
            if (containers.ContainsKey(functionClassName))
            {
                var container = containers[functionClassName];
                var scope = instanceContainers.GetOrAdd(functionInstanceId, id => container.BeginLifetimeScope());

                object resolved = null;
                if (string.IsNullOrWhiteSpace(name))
                {
                    resolved = scope.Resolve(type);
                }
                else
                {
                    resolved = scope.ResolveNamed(name, type);
                }
                return resolved;
            }
            else
            {
                throw new InitializationException("DependencyInjection.Initialize must be called before dependencies can be resolved.");
            }
        }

        public static void RemoveScope(Guid functionInstanceId)
        {
            if (instanceContainers.TryRemove(functionInstanceId, out ILifetimeScope scope))
            {
                scope.Dispose();
            };
        }

        /// <summary>
        /// Verifies that the depency injection is set up properly for the given type. Searches for public static functions.
        /// Verifies the following things:
        /// * That an InjectAttribute on a parameter, has a matching DependencyInjectionConfigAttribute on the class.
        /// * That the configuration can be constructed with a string-parameter.
        /// * That the injected parameters can be resolved using the given configuration.
        /// * Optionally that a DependencyInjectionConfigAttribute has at least one InjectAttribute on a method.
        /// </summary>
        /// <param name="type">The type to verify.</param>
        /// <param name="verifyUnnecessaryConfig">If true, verify that no configuration exists unless there is at least one injected parameter. Defaults to true.</param>
        public static void VerifyConfiguration(Type type, bool verifyUnnecessaryConfig = true)
        {
            var configAttr = type.GetCustomAttribute<DependencyInjectionConfigAttribute>();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            var injectAttrFound = false;

            foreach (var method in methods)
            {
                foreach (var param in method.GetParameters())
                {
                    var injectAttr = param.GetCustomAttribute<InjectAttribute>();

                    if (injectAttr == null)
                    {
                        continue;
                    }

                    if (configAttr == null)
                    {
                        throw new MissingAttributeException($"{nameof(InjectAttribute)} used without {nameof(DependencyInjectionConfigAttribute)}");
                    }

                    injectAttrFound = true;
                    var functionName = $"testfunction-{Guid.NewGuid()}";
                    Activator.CreateInstance(configAttr.Config, functionName);
                    var functionInstanceId = Guid.NewGuid();
                    Resolve(param.ParameterType, injectAttr.Name, functionName, functionInstanceId);
                }
            }

            if (!injectAttrFound && configAttr != null && verifyUnnecessaryConfig)
            {
                throw new MissingAttributeException($"{nameof(DependencyInjectionConfigAttribute)} used without {nameof(InjectAttribute)}");
            }
        }
    }
}
