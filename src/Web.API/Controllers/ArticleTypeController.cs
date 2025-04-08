using Application.OperationResults;
using Application.UseCases.ArticleTypes.Commands.Create;
using Application.UseCases.ArticleTypes.Commands.Delete;
using Application.UseCases.ArticleTypes.Commands.Update;
using Application.UseCases.ArticleTypes.Queries.GetAll;
using Application.UseCases.ArticleTypes.Queries.GetById;
using Domain.Entities.ArticleTypes;
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
        [ProducesResponseType(typeof(IEnumerable<ArticleType>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var operationResult = await _mediator.Send(new ArticleTypeGetAllQuery());
            return HandleResponse(operationResult);
        }

        [HttpGet(routeTemplateId)]
        [ProducesResponseType(typeof(ArticleType), (int)HttpStatusCode.OK)]
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
