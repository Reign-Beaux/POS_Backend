using Application.OperationResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Web.API.Controllers.Base
{
    [ApiController]
    public class ControllerAbstraction : ControllerBase
    {

        protected IActionResult HandleResponse<T>(OperationResult<T> operationResult)
        {
            if (operationResult.IsSuccess)
            {
                return Ok(operationResult.Value);
            }

            Dictionary<HttpStatusCode, Func<ErrorDetails, IActionResult>> failureDictionary = new()
            {
                { HttpStatusCode.BadRequest, BadRequest },
                { HttpStatusCode.NotFound, NotFound },
                { HttpStatusCode.InternalServerError, details => StatusCode((int)details.Status, details.Message) },
                { HttpStatusCode.Unauthorized, Unauthorized }
            };

            return failureDictionary[operationResult.ProblemDetails!.Status](operationResult.ProblemDetails!);
        }
    }
}
