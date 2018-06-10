using System;
using Microsoft.Azure.WebJobs.Description;

namespace AzureFunctions.Autofac.Attributes
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
