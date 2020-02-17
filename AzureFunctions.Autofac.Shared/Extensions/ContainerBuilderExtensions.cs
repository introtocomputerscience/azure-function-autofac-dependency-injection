using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Autofac.Shared.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterLoggerFactory(this ContainerBuilder builder, ILoggerFactory factory)
        {
            builder.RegisterInstance(factory).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
        }
    }
}
