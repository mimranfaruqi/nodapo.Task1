using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nodapo.Filters
{
    /// <summary>
    /// This is a middleware that filters every request and only validated request get passed. All others are returned
    /// back to the server.
    /// </summary>
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key,
                        kvp => kvp.Value?.Errors
                            .Select(x => x.ErrorMessage))
                    .ToArray();

                var errorResponse = new List<Tuple<string, string>>();

                foreach (var error in errorsInModelState)
                {
                    foreach (var subError in error.Value)
                    {
                        var errorModel = new Tuple<string, string>
                        (
                            error.Key,
                            subError
                        );

                        errorResponse.Add(errorModel);
                    }
                }

                context.Result = new BadRequestObjectResult(errorResponse);

                return;
            }

            await next();
        }
    }
}