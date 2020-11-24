using AzureFunctions.Autofac.Configuration;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AzureFunctions.Autofac.Provider.Config
{
    public class ScopeFilter : IFunctionInvocationFilter, IFunctionExceptionFilter, IFunctionFilter
    {
        public Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken)
        {
            RemoveScope(exceptionContext.FunctionInstanceId);
            return Task.CompletedTask;
        }

        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            RemoveScope(executedContext.FunctionInstanceId);
            return Task.CompletedTask;
        }

        public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void RemoveScope(Guid functionInstanceId)
        {
            DependencyInjection.RemoveScope(functionInstanceId);
        }
    }
}

