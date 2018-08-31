using Autofac;
using Autofac.Core;
using AzureFunctions.Autofac.Configuration;
using AzureFunctions.Autofac.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureFunctions.Autofac.NetFramework.Tests
{
    [TestClass]
    public class DependencyInjectionVerificationTests
    {
        [TestMethod]
        public void VerifyConfiguration_Should_PassSuccessfully_When_CorrectConfiguration()
        {
            DependencyInjection.VerifyConfiguration(typeof(FunctionWithCorrectConfiguration));
        }

        [TestMethod]
        public void VerifyConfiguration_Should_ThrowException_When_IncorrectConfiguration()
        {
            Assert.ThrowsException<DependencyResolutionException>(() =>
                DependencyInjection.VerifyConfiguration(typeof(FunctionWithIncorrectConfiguration)));
        }

        [TestMethod]
        public void VerifyConfiguration_Should_ThrowException_When_MisingConfiguration()
        {
            Assert.ThrowsException<MissingAttributeException>(() =>
                DependencyInjection.VerifyConfiguration(typeof(FunctionWithInjectionButNoConfiguration)));
        }

        [TestMethod]
        public void VerifyConfiguration_Should_ThrowException_When_MisingInjectAttribute()
        {
            Assert.ThrowsException<MissingAttributeException>(() =>
                DependencyInjection.VerifyConfiguration(typeof(FunctionWithConfigurationButNoInjection)));
        }

        [TestMethod]
        public void VerifyConfiguration_Should_Pass_When_NotRequiringAMatchingAttribute()
        {
            DependencyInjection.VerifyConfiguration(typeof(FunctionWithConfigurationButNoInjection), false);
        }

        [TestMethod]
        public void VerifyConfiguration_Should_Pass_When_NoConfigurationOrAttributes()
        {
            DependencyInjection.VerifyConfiguration(typeof(FunctionWithNeitherConfigurationNorInjection));
        }

        // Interface/Implementation
        public interface IInterface1 { }
        public interface IInterface2 { }

        public class DependentImplementation : IInterface1
        {
            public DependentImplementation(IInterface2 _) { }
        }

        public class DependeeImplementation : IInterface2 { }

        // Configurations
        public class CorrectConfigurationExample
        {
            public CorrectConfigurationExample(string functionName)
            {
                DependencyInjection.Initialize(builder =>
                {
                    builder.RegisterType<DependentImplementation>().As<IInterface1>();
                    builder.RegisterType<DependeeImplementation>().As<IInterface2>();
                }, functionName);
            }
        }

        public class IncorrectConfigurationExample
        {
            public IncorrectConfigurationExample(string functionName)
            {
                DependencyInjection.Initialize(builder =>
                {
                    builder.RegisterType<DependentImplementation>().As<IInterface1>();
                }, functionName);
            }
        }

        // Example classes using dependency injection
        [DependencyInjectionConfig(typeof(CorrectConfigurationExample))]
        public class FunctionWithCorrectConfiguration
        {
            public static void Run([Inject] IInterface1 d) { }
        }

        [DependencyInjectionConfig(typeof(IncorrectConfigurationExample))]
        public class FunctionWithIncorrectConfiguration
        {
            public static void Run([Inject] IInterface1 d) { }
        }

        public class FunctionWithInjectionButNoConfiguration
        {
            public static void Run([Inject] IInterface1 d) { }
        }

        [DependencyInjectionConfig(typeof(CorrectConfigurationExample))]
        public class FunctionWithConfigurationButNoInjection
        {
            public static void Run() { }
        }

        public class FunctionWithNeitherConfigurationNorInjection
        {
            public static void Run() { }
        }
    }
}
