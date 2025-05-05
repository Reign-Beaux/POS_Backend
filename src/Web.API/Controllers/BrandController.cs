using Application.Features.Brands.UseCases.Commands.Create;
using Application.Features.Brands.UseCases.Queries.GetById;
using Application.OperationResults;
using Application.Shared.Catalogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Web.API.Controllers.Base;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    public class BrandController(ISender sender) : ControllerAbstraction
    {
        private const string routeTemplateId = "{id:Guid}";

        [HttpGet(routeTemplateId)]
        [ProducesResponseType(typeof(CatalogDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            //BrandGetByIdQuery query = new(id);
            //var operationResult = await sender.Send(query);

            //if (operationResult.Status != HttpStatusCode.OK)
            //    return HandleErrorResponse(operationResult);

            //return Ok(operationResult.Value);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BrandCreateCommand command)
        {
            var operationResult = await sender.Send(command);

            if (operationResult.Status != HttpStatusCode.Created)
                return HandleErrorResponse(operationResult);

            return CreatedAtAction(nameof(GetById), new { id = operationResult.Value }, operationResult.Value);
        }
    }
}
