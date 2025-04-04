using Domain.Entities.ArticleTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Web.API.Controllers.Base;

namespace Web.API.Controllers
{
    public class ArticleTypeController(ISender mediator) : ControllerAbstraction
    {
        private readonly ISender _mediator = mediator;
        private const string routeTemplateId = "{id:Guid}";

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleType>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            //var operationResult = await _mediator.Send(new GetAllArticleTypesQuery());
            //return HandleResponse(operationResult);
        }
    }
}
