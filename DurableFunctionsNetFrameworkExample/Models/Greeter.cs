using DurableFunctionsNetFrameworkExample.Interfaces;

namespace DurableFunctionsNetFrameworkExample.Models
{
    public class Greeter : IGreeter
    {
        public string Greet(string name) => $"Hello {name}!";
    }
}
