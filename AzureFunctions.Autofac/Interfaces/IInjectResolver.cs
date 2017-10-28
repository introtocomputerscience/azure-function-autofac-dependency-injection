using Microsoft.Azure.WebJobs.Host.Bindings;
using System;

namespace AzureFunctions.Autofac
{
    public interface IInjectResolver
    {
        object Resolve(Type type);
    }
}
