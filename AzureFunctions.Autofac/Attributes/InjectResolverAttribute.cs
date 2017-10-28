using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctions.Autofac
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InjectResolverAttribute : Attribute
    {
        public Type Resolver { get; }

        public InjectResolverAttribute(Type resolver)
        {
            Resolver = resolver;
        }
    }
}
