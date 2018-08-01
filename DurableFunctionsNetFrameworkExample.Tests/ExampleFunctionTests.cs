using DurableFunctionsNetFrameworkExample.Functions;
using DurableFunctionsNetFrameworkExample.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace DurableFunctionsNetFrameworkExample.Tests
{
    [TestClass]
    public class ExampleFunctionTests
    {
        [TestMethod]
        public async Task GreeterFunction_ShouldProduceExpectedOutput()
        {
            var durableOrchestrationContextBase = new Mock<DurableOrchestrationContextBase>();
            var loggerMock = new Mock<ILogger>();
            durableOrchestrationContextBase.Setup(x => x.GetInput<string>()).Returns("Test");
            durableOrchestrationContextBase.Setup(x => x.CallActivityAsync<string>("PrimaryGoodbye", It.IsAny<string>())).Returns((string functionName, string name) => ExampleFunction.PrimaryGoodbye(name, new Goodbyer()));
            durableOrchestrationContextBase.Setup(x => x.CallActivityAsync<string>("SecondaryGoodbye", It.IsAny<string>())).Returns((string functionName, string name) => ExampleFunction.SecondaryGoodbye(name, new AlternateGoodbyer()));
            var result = await ExampleFunction.Run(durableOrchestrationContextBase.Object, loggerMock.Object, new Greeter());
            Assert.AreEqual("Hello Test! So long Test... or Farewell Test!", result);
        }
    }
}
