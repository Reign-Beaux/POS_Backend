using Application.OperationResults;
using MediatR;

namespace Application.Features.Brands.Commands.Delete
{
    public record BrandDeleteCommand(Guid Id) : IRequest<OperationResult<Unit>>;
}
