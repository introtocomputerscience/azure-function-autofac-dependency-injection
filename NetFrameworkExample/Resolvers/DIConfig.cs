using Autofac;
using AutofacDIExample.Modules;
using AzureFunctions.Autofac;
using AzureFunctions.Autofac.Configuration;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;

namespace AutofacDIExample.Resolvers
{
    public class DIConfig
    {
        public DIConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterModule(new TestModule());
            }, functionName);
        }
    }
}
