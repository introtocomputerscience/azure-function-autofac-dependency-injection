using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctions.Autofac
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
