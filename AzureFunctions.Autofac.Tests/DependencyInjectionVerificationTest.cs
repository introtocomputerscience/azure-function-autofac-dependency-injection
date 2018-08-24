using Autofac;
using Autofac.Core;
using AzureFunctions.Autofac.Configuration;
using AzureFunctions.Autofac.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureFunctions.Autofac.Tests
{
    [TestClass]
    public class DependencyInjectionVerificationTest
    {
        [TestMethod]
        public void TestCorrectConfiguration()
        {
            DependencyInjection.VerifyConfiguration(typeof(FunctionWithCorrectConfiguration));
        }

        [TestMethod]
        public void TestIncorrectConfiguration()
        {
            Assert.ThrowsException<DependencyResolutionException>(() =>
                DependencyInjection.VerifyConfiguration(typeof(FunctionWithIncorrectConfiguration)));
        }

        [TestMethod]
        public void TestMissingConfiguration()
        {
            Assert.ThrowsException<MissingAttributeException>(() =>
                DependencyInjection.VerifyConfiguration(typeof(FunctionWithInjectionButNoConfiguration)));
        }

        [TestMethod]
        public void TestMissingInjection()
        {
            Assert.ThrowsException<MissingAttributeException>(() =>
                DependencyInjection.VerifyConfiguration(typeof(FunctionWithConfigurationButNoInjection)));
        }

        [TestMethod]
        public void TestMissingInjectionCanBeIgnored()
        {
            DependencyInjection.VerifyConfiguration(typeof(FunctionWithConfigurationButNoInjection), false);
        }

        [TestMethod]
        public void TestTypeWithNeitherConfigurationNorInjection()
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
