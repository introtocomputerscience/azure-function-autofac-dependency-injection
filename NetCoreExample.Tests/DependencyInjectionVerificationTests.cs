using AzureFunctions.Autofac.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NetCoreExample.Functions;
using System;
using Xunit;

namespace NetCoreExample.Tests
{
    public class DependencyInjectionVerificationTests
    {
        [Fact]
        public void VerifyConfiguration_Should_PassSuccessfully_When_ILoggerFactory_Required()
        {
            var loggerFactory = new Mock<ILoggerFactory>();
            DependencyInjection.VerifyConfiguration(typeof(LoggerFunction), loggerFactory: loggerFactory.Object);
        }
    }
}
