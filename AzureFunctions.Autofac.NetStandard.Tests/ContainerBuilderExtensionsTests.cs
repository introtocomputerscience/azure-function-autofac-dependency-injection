using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
using AzureFunctions.Autofac.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AzureFunctions.Autofac.NetStandard.Tests
{
    [TestClass]
    public class ContainerBuilderExtensionsTests
    {
        [TestMethod]
        public void ContainerBuilderExtension_Should_AllowToProvideILogger_When_LoggerFactoryIsGivenToTheContainer()
        {
            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerFactory
                .Setup(lf => lf.CreateLogger("AzureFunctions.Autofac.NetStandard.Tests.ContainerBuilderExtensionsTests.TestService"))
                .Returns(logger.Object);

            var builder = new ContainerBuilder();
            builder.RegisterLoggerFactory(loggerFactory.Object);
            builder.RegisterType<TestService>();
            var container = builder.Build();

            // In order to provide an instance of TestService, the container has to build a Logger<>,
            // which requires a properly configured ILoggerFactory instance
            var service = container.Resolve<TestService>();

            Assert.IsNotNull(service.Logger);
            Assert.IsInstanceOfType(service.Logger, typeof(Logger<TestService>));

            // The factory should have been requested to populate the inner logger of the Logger<> wrapper,
            // to ensure it uses the config & providers setup for the app
            loggerFactory
                .Verify(
                    lf => lf.CreateLogger(
                        "AzureFunctions.Autofac.NetStandard.Tests.ContainerBuilderExtensionsTests.TestService"),
                    Times.Once);
        }

        [TestMethod]
        public void ContainerBuilderExtension_Should_FailToProvideILogger_When_NoLoggerFactoryIsGivenToTheContainer()
        {
            var builder = new ContainerBuilder();
            // Register only TestService, but do not give any ILoggerFactory to the container
            builder.RegisterType<TestService>();
            var container = builder.Build();

            var ex = Assert.ThrowsException<DependencyResolutionException>(() => container.Resolve<TestService>());
            Assert.IsTrue(ex.Message.Contains("TestService"));
        }

        private class TestService
        {
            public ILogger<TestService> Logger { get; }

            public TestService(ILogger<TestService> logger)
            {
                Logger = logger;
            }
        }
    }
}
