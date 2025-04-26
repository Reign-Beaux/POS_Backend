using Application.DTOs.ArticleTypes;
using Application.OperationResults;
using MediatR;

namespace Application.Features.ArticleTypes.Queries.GetAll
{
    public record ArticleTypeGetAllQuery() : IRequest<OperationResult<IEnumerable<ArticleTypeDTO>>>;
}
