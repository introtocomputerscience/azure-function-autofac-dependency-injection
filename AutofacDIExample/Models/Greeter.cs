using AutofacDIExample.Interfaces;

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
