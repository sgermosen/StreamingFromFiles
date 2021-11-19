using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EngineAPI.Behaviors
{
    public static class BehaviorBadRequests
    {
        public static void Parse(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = actionContex =>
            {
                var response = new List<string>();
                foreach (var key in actionContex.ModelState.Keys)
                {
                    foreach (var error in actionContex.ModelState[key].Errors)
                    {
                        response.Add($"{key}: {error.ErrorMessage}");
                    }
                }
                return new BadRequestObjectResult(response);
            };
        }
    }
}
