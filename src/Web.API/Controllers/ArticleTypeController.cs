using Application.UseCases.ArticleTypes.Commands.Create;
using Application.UseCases.ArticleTypes.Queries.GetAll;
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Create([FromBody] ArticleTypeCreateCommand command)
        {
            var operationResult = await _mediator.Send(command);
            return HandleResponse(operationResult);
        }
    }
}
