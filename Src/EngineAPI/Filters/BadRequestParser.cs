using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace EngineAPI.Filters
{
    public class BadRequestParser : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var castResult = context.Result as IStatusCodeActionResult;

            var statusCode = castResult?.StatusCode;
            if (statusCode != 400) return;
            var response = new List<string>();
            var actualResult = context.Result as BadRequestObjectResult;
            if (actualResult?.Value is string)
                response.Add(actualResult.Value.ToString());
            else if (actualResult is { Value: IEnumerable<IdentityError> errors })
                response.AddRange(errors.Select(error => error.Description));
            else
                response.AddRange(from key in context.ModelState.Keys from error in context.ModelState[key].Errors select $"{key}: {error.ErrorMessage}");

            context.Result = new BadRequestObjectResult(response);

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
