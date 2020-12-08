using System;
using System.Threading.Tasks;

namespace caching_example.Interfaces
{
    public interface ICacheTester
    {
        Task<string> GetMessage(DateTime executionTime, int sleepTimeInMillis);
    }
}
