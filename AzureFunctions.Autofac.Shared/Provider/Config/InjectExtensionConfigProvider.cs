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
        private InjectBindingProvider _bindingProvider;

#if NET46
        public InjectExtensionConfigProvider() { }
#endif

#if NETSTANDARD2_0
        public InjectExtensionConfigProvider(IOptions<ExecutionContextOptions> options, ILoggerFactory loggerFactory)
        {
            var appDirectory = options.Value.AppDirectory;
            this._bindingProvider = new InjectBindingProvider(appDirectory, loggerFactory);
        }
#endif

        public void Initialize(ExtensionConfigContext context)
        {
#if NET46
            this._bindingProvider = new InjectBindingProvider(null,context.Config.LoggerFactory);
#endif
            context.AddBindingRule<InjectAttribute>().Bind(this._bindingProvider);
        }
    }
}

