using Microsoft.Extensions.Logging;

namespace AutofacDIExample
{
    public class Goodbyer : IGoodbyer
    {
        private readonly ILogger<Goodbyer> logger;

        public Goodbyer(ILogger<Goodbyer> logger)
        {
            this.logger = logger;
        }

        public string Goodbye()
        {
            // To see this in the Azure Function CLI window you need to provide a logging configuration in host.json 
            //    that enables the warning level either for everything or for this class
            //     For more information see 
            //     https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#configuration
            logger.LogWarning("Saying goodbye");

            return "So long...";
        }
    }
}
