using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutofacDIExample.DependencyInjection
{
    public class InjectValueProvider : IValueProvider
    {
        private readonly object value;
        public InjectValueProvider(object value) => this.value = value;
        public Type Type => this.value.GetType();

        public Task<object> GetValueAsync() => Task.FromResult(this.value);

        public string ToInvokeString() => this.value.ToString();
    }
}
