using Application.OperationResults;
using Application.Features.Brands.UseCases.Commands.Delete;
using Application.Features.Brands.UseCases.Commands.Update;
using Application.Features.Brands.UseCases.Queries.GetAll;
using Application.Features.Brands.UseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Web.API.Controllers.Base;
using Application.Features.Brands.DTOs;
using Application.Features.Brands.UseCases.Commands.Create;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    public class BrandController(ISender sender) : ControllerAbstraction
    {
        private const string routeTemplateId = "{id:Guid}";

        //[HttpGet]
        //[ProducesResponseType(typeof(IEnumerable<BrandDTO>), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> GetAll()
        //{
        //    var operationResult = await sender.Send(new BrandGetAllQuery());
        //    return HandleResponse(operationResult);
        //}

        [HttpGet(routeTemplateId)]
        [ProducesResponseType(typeof(BrandDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            BrandGetByIdQuery query = new(id);
            var operationResult = await sender.Send(query);

            if (operationResult.Status != HttpStatusCode.OK)
                return HandleErrorResponse(operationResult);

            return Ok(operationResult.Value);
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

        //[HttpPut]
        //[ProducesResponseType((int)HttpStatusCode.NoContent)]
        //[ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
        //public async Task<IActionResult> Update([FromBody] BrandUpdateCommand command)
        //{
        //    var operationResult = await sender.Send(command);
        //    return HandleResponse(operationResult);
        //}

        //[HttpDelete(routeTemplateId)]
        //[ProducesResponseType((int)HttpStatusCode.NoContent)]
        //[ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var operationResult = await sender.Send(new BrandDeleteCommand(id));
        //    return HandleResponse(operationResult);
        //}
    }
}
