using Application.DTOs.ArticleTypes;
using Application.OperationResults;
using Application.Features.ArticleTypes.Commands.Create;
using Application.Features.ArticleTypes.Commands.Delete;
using Application.Features.ArticleTypes.Commands.Update;
using Application.Features.ArticleTypes.Queries.GetAll;
using Application.Features.ArticleTypes.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Web.API.Controllers.Base;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    public class ArticleTypeController(ISender mediator) : ControllerAbstraction
    {
        private readonly ISender _mediator = mediator;
        private const string routeTemplateId = "{id:Guid}";

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleTypeDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var operationResult = await _mediator.Send(new ArticleTypeGetAllQuery());
            return HandleResponse(operationResult);
        }

        [HttpGet(routeTemplateId)]
        [ProducesResponseType(typeof(ArticleTypeDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            ArticleTypeGetByIdQuery query = new(id);
            var operationResult = await _mediator.Send(query);
            return HandleResponse(operationResult);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Create([FromBody] ArticleTypeCreateCommand command)
        {
            var operationResult = await _mediator.Send(command);
            return HandleResponse(operationResult);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update([FromBody] ArticleTypeUpdateCommand command)
        {
            var operationResult = await _mediator.Send(command);
            return HandleResponse(operationResult);
        }

        [HttpDelete(routeTemplateId)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var operationResult = await _mediator.Send(new ArticleTypeDeleteCommand(id));
            return HandleResponse(operationResult);
        }
    }
}
