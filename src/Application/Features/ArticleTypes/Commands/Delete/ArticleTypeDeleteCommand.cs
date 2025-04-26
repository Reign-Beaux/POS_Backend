using Application.OperationResults;
using MediatR;

namespace Application.Features.ArticleTypes.Commands.Delete
{
    public record ArticleTypeDeleteCommand(Guid Id) : IRequest<OperationResult<Unit>>;
}
