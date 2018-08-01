using DurableFunctionsNetFrameworkExample.Interfaces;

namespace DurableFunctionsNetFrameworkExample.Models
{
    public class AlternateGoodbyer : IGoodbyer
    {
        public string Goodbye(string name) => $"Farewell {name}!";
    }
}
