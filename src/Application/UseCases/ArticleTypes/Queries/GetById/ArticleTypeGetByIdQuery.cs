using Application.OperationResults;
using Domain.Entities.ArticleTypes;
using MediatR;

namespace Application.UseCases.ArticleTypes.Queries.GetById
{
    public record ArticleTypeGetByIdQuery(Guid Id) : IRequest<OperationResult<ArticleType>>;
}
