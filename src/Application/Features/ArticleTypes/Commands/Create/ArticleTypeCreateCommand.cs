using Application.OperationResults;
using MediatR;

namespace Application.Features.ArticleTypes.Commands.Create
{
    public record ArticleTypeCreateCommand(
        string Name,
        string Description) : IRequest<OperationResult<Unit>>;
}
