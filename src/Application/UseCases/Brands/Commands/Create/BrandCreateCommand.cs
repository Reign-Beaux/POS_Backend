using Application.OperationResults;
using MediatR;

namespace Application.UseCases.Brands.Commands.Create
{
    public record BrandCreateCommand(
        string Name,
        string Description) : IRequest<OperationResult<Unit>>;
}
