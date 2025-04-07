using Application.OperationResults;
using MediatR;

namespace Application.UseCases.ArticleTypes.Commands.Create
{
    public record ArticleTypeCreateCommand(
        string Name,
        string Description) : IRequest<OperationResult<Unit>>;
}
