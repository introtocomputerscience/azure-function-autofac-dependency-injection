using Autofac;
using AutofacDIExample.Modules;
using AzureFunctions.Autofac;
using AzureFunctions.Autofac.Configuration;
using AzureFunctions.Autofac.Shared.Extensions;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;
using NetCoreExample.Interfaces;
using NetCoreExample.Models;
using System;

namespace AutofacDIExample.Resolvers
{
    public class DIConfig
    {
        public DIConfig(string functionName, string baseDirectory, ILoggerFactory factory)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterModule(new TestModule());
                builder.Register<DirectoryAnnouncer>(c => new DirectoryAnnouncer(baseDirectory)).As<IAnnouncer>();
                builder.RegisterLoggerFactory(factory);
            }, functionName);
        }
    }
}
