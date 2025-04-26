using Application.OperationResults;
using MediatR;

namespace Application.Features.Brands.UseCases.Commands.Create
{
    public record BrandCreateCommand(
        string Name,
        string Description) : IRequest<OperationResult<Guid>>;
}
