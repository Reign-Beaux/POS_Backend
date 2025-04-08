using Application.OperationResults;
using MediatR;

namespace Application.UseCases.ArticleTypes.Commands.Delete
{
    public record ArticleTypeDeleteCommand(Guid Id) : IRequest<OperationResult<Unit>>;
}
