using Application.Features.Brands.DTOs;
using Application.OperationResults;
using MediatR;

namespace Application.Features.Brands.UseCases.Commands.Create
{
    public record BrandCreateCommand : BrandDTO, IRequest<OperationResult<Guid>>;
}
