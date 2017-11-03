using Microsoft.Azure.WebJobs.Description;
using System;

namespace AzureFunctions.Autofac
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
        public String Name { get; }

        public InjectAttribute(String name = null)
        {
            Name = name;
        }
    }
}
