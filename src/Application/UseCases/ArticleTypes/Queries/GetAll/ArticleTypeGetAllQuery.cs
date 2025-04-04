using Application.OperationResults;
using Domain.Entities.ArticleTypes;
using MediatR;

namespace Application.UseCases.ArticleTypes.Queries.GetAll
{
    public record ArticleTypeGetAllQuery() : IRequest<OperationResult<IEnumerable<ArticleType>>>;
}
