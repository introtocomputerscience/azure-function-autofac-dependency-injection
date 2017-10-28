using AutofacDIExample.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutofacDIExample.Models
{
    public class Greeter : IGreeter
    {
        public string Greet()
        {
            return "Hello Programmer!";
        }
    }
}
