using Application.OperationResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Web.API.Controllers.Base
{
    [ApiController]
    public class ControllerAbstraction : ControllerBase
    {

        protected IActionResult HandleErrorResponse<T>(OperationResult<T> operationResult)
        {
            Dictionary<HttpStatusCode, Func<ErrorDetails, IActionResult>> failureDictionary = new()
            {
                { HttpStatusCode.BadRequest, BadRequest },
                { HttpStatusCode.Conflict, Conflict },
                { HttpStatusCode.InternalServerError, details => StatusCode((int)operationResult.Status, details.Message) },
                { HttpStatusCode.NotFound, NotFound },
                { HttpStatusCode.Unauthorized, Unauthorized }
            };

            if (failureDictionary.TryGetValue(operationResult.Status, out var handler))
            {
                return handler(operationResult.ErrorDetails!);
            }

            return StatusCode((int)operationResult.Status, operationResult.ErrorDetails!.Message);
        }
    }
}
