using Autofac;
using AzureFunctions.Autofac.Exceptions;
using System;
using System.Linq.Expressions;

namespace AzureFunctions.Autofac.Configuration
{
    public static class DependencyInjection
    {
        private static bool initialized = false;
        private static IContainer container;
        public static void Initialize(Action<ContainerBuilder> cfg)
        {
            if (!initialized)
            {
                ContainerBuilder builder = new ContainerBuilder();
                cfg(builder);
                container = builder.Build();
                initialized = true;
            }
        }

        public static object Resolve(Type type, string name)
        {
            if (!initialized) { throw new InitializationException("DependencyInjection.Initialize must be called before dependencies can be resolved."); }
            object resolved = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                resolved = container.Resolve(type);
            }
            else
            {
                resolved = container.ResolveNamed(name, type);
            }
            return resolved;
        }
    }
}
