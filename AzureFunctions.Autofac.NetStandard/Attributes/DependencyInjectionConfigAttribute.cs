using System;

namespace AzureFunctions.Autofac.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyInjectionConfigAttribute : Attribute
    {
        public Type Config { get; }

        public DependencyInjectionConfigAttribute(Type config)
        {
            Config = config;
        }
    }
}
