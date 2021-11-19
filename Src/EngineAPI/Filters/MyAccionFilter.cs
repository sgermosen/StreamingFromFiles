using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace EngineAPI.Filters
{
    public class MyAccionFilter : IActionFilter
    {
        private readonly ILogger<MyAccionFilter> logger;

        public MyAccionFilter(ILogger<MyAccionFilter> logger)
        {
            this.logger = logger;
        }
        //this is the first, but visualy it use to come later
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // logger.LogInformation("Before execute an acction");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // logger.LogInformation("after execute an acction"); 
        }
    }
}
