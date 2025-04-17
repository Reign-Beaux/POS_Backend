using Application.DTOs.Brands;
using Application.OperationResults;
using Application.UseCases.Brands.Commands.Create;
using Application.UseCases.Brands.Commands.Delete;
using Application.UseCases.Brands.Commands.Update;
using Application.UseCases.Brands.Queries.GetAll;
using Application.UseCases.Brands.Queries.GetById;
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

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BrandDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var operationResult = await sender.Send(new BrandGetAllQuery());
            return HandleResponse(operationResult);
        }

        [HttpGet(routeTemplateId)]
        [ProducesResponseType(typeof(BrandDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            BrandGetByIdQuery query = new(id);
            var operationResult = await sender.Send(query);
            return HandleResponse(operationResult);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Create([FromBody] BrandCreateCommand command)
        {
            var operationResult = await sender.Send(command);
            return HandleResponse(operationResult);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update([FromBody] BrandUpdateCommand command)
        {
            var operationResult = await sender.Send(command);
            return HandleResponse(operationResult);
        }

        [HttpDelete(routeTemplateId)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var operationResult = await sender.Send(new BrandDeleteCommand(id));
            return HandleResponse(operationResult);
        }
    }
}
