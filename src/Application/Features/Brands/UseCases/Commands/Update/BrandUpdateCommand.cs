using Application.OperationResults;
using MediatR;

namespace Application.Features.Brands.UseCases.Commands.Update
{
    public record BrandUpdateCommand(
        Guid Id,
        string Name,
        string Description) : IRequest<OperationResult<Unit>>;
}
