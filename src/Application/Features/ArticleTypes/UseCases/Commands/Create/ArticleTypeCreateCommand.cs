using Application.Features.ArticleTypes.DTOs;
using Application.OperationResults;
using MediatR;

namespace Application.Features.ArticleTypes.UseCases.Commands.Create
{
    public record ArticleTypeCreateCommand : ArticleTypeDTO, IRequest<OperationResult<Guid>>;
}
