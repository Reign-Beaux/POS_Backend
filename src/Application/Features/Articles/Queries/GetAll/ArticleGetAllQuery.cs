using Application.DTOs.Articles;
using Application.OperationResults;
using MediatR;

namespace Application.Features.Articles.Queries.GetAll
{
    public record ArticleGetAllQuery : IRequest<OperationResult<IEnumerable<ArticleDTO>>>;
}
