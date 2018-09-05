namespace AzureFunctions.Autofac
{
    using System;

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