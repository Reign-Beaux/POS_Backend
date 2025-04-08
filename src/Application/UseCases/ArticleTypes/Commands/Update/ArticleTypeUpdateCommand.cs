using Application.OperationResults;
using MediatR;

namespace Application.UseCases.ArticleTypes.Commands.Update
{
    public record ArticleTypeUpdateCommand(
        Guid Id,
        string Name,
        string Description) : IRequest<OperationResult<Unit>>;
}
