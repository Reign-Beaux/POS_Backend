using Application.Features.Brands.DTOs;
using Application.OperationResults;
using MediatR;

namespace Application.Features.Brands.UseCases.Commands.Update
{
    public record BrandUpdateCommand : BrandDTO, IRequest<OperationResult<Unit>>;
}
