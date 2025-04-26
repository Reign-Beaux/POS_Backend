using Application.OperationResults;
using MediatR;

namespace Application.Features.Brands.Commands.Create
{
    public record BrandCreateCommand(
        string Name,
        string Description) : IRequest<OperationResult<string>>;
}
