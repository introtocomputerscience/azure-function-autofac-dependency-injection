using Microsoft.Extensions.Logging;

namespace FunctionsV3Example
{
    public class LogWriter : ILogWriter
    {
        private readonly ILogger<LogWriter> _logger;

        public LogWriter(ILogger<LogWriter> logger)
        {
            _logger = logger;
        }

        public void Log()
        {
            _logger.LogInformation("Hello Developer!");
        }
    }
}