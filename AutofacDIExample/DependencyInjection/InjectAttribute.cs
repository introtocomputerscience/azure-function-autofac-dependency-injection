using Microsoft.Azure.WebJobs.Description;
using System;

namespace AutofacDIExample.DependencyInjection
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}
