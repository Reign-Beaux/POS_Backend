using Application.DTOs.Articles;
using Application.Features.Articles.Commands.Create;
using Application.Features.Articles.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Web.API.Controllers.Base;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    public class ArticleController(ISender sender) : ControllerAbstraction
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var operationResult = await sender.Send(new ArticleGetAllQuery());
            return HandleResponse(operationResult);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Create([FromBody] ArticleCreateCommand command)
        {
            var operationResult = await sender.Send(command);
            return HandleResponse(operationResult);
        }
    }
}
