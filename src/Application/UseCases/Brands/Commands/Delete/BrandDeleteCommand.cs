using Application.OperationResults;
using MediatR;

namespace Application.UseCases.Brands.Commands.Delete
{
    public record BrandDeleteCommand(Guid Id) : IRequest<OperationResult<Unit>>;
}
