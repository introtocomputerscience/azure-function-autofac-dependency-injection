using AzureFunctions.Autofac.Configuration;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AzureFunctions.Autofac
{
  public class ScopeFilterAttribute : FunctionInvocationFilterAttribute
  {
    public override Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
    {
      RemoveScope(executedContext.FunctionInstanceId);
      return Task.CompletedTask;
    }

    public override Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }

    private static void RemoveScope(Guid functionInstanceId)
    {
      DependencyInjection.RemoveScope(functionInstanceId);
    }
  }
}
