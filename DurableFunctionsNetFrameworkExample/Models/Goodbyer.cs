using DurableFunctionsNetFrameworkExample.Interfaces;

namespace DurableFunctionsNetFrameworkExample.Models
{
    public class Goodbyer : IGoodbyer
    {
        public string Goodbye(string name) => $"So long {name}...";
    }
}
