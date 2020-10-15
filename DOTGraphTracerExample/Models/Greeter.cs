using DOTGraphTracerExample.Interfaces;

namespace DOTGraphTracerExample.Models
{
    public class Greeter : IGreeter
    {
        public string Greet(string name)
        {
            return $"Hello {name}, nice to meet you!";
        }
    }
}
