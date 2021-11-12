using Microsoft.Extensions.Logging;

namespace BgServicex
{
    public interface IServiceB
    {
        void Run();
    }

    public class ServiceB : IServiceB
    {
        private readonly ILogger<ServiceB> _logger;

        public ServiceB(ILogger<ServiceB> logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("In Service B");
        }
    }
}
