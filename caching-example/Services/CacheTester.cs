using caching_example.Interfaces;
using System;
using System.Threading.Tasks;

namespace caching_example.Services
{
    public class CacheTester : ICacheTester
    {
        private readonly Guid _guid;

        public CacheTester()
        {
            _guid = Guid.NewGuid();
        }
        public async Task<string> GetMessage(DateTime executionTime, int sleepTimeInMillis)
        {
            await Task.Delay(sleepTimeInMillis);
            return $"At {executionTime} we got {_guid}";
        }
    }
}
