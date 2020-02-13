using Autofac;
using Microsoft.Azure.WebJobs.Host.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

#if NETSTANDARD2_0
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endif

namespace AzureFunctions.Autofac.Provider.Config
{
    public class InjectExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly InjectBindingProvider bindingProvider;

#if NET46
        public InjectExtensionConfigProvider()
        {
            this.bindingProvider = new InjectBindingProvider();
        }
#endif

#if NETSTANDARD2_0
        public InjectExtensionConfigProvider(IOptions<ExecutionContextOptions> options, ILoggerFactory loggerFactory)
        {
            var appDirectory = options.Value.AppDirectory;
            this.bindingProvider = new InjectBindingProvider(appDirectory, loggerFactory);
        }
#endif

        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<InjectAttribute>().Bind(this.bindingProvider);
        }
    }
}

