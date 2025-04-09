using Application.DTOs.ArticleTypes;
using Application.OperationResults;
using MediatR;

namespace Application.UseCases.ArticleTypes.Queries.GetAll
{
    public record ArticleTypeGetAllQuery() : IRequest<OperationResult<IEnumerable<ArticleTypeDTO>>>;
}
