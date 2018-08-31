using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Autofac
{
    public class InjectValueProvider : IValueProvider
    {
        private readonly object value;

        public InjectValueProvider(object value) => this.value = value;

        public Type Type => value.GetType();

        public Task<object> GetValueAsync() => Task.FromResult(value);

        public string ToInvokeString() => value.ToString();
    }
}
