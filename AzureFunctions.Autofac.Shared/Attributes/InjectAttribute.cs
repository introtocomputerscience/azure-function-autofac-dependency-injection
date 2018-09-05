using System;
using Microsoft.Azure.WebJobs.Description;

namespace AzureFunctions.Autofac
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class InjectAttribute : Attribute
    {
        public string Name { get; }

        public InjectAttribute(string name = null)
        {
            Name = name;
        }
    }
}
