using Application.DTOs.ArticleTypes;
using Application.OperationResults;
using MediatR;

namespace Application.Features.ArticleTypes.Queries.GetById
{
    public record ArticleTypeGetByIdQuery(Guid Id) : IRequest<OperationResult<ArticleTypeDTO>>;
}
