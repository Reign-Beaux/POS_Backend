using Application.Features.Brands.DTOs;
using Application.OperationResults;
using MediatR;

namespace Application.Features.Brands.Queries.GetById
{
    public record BrandGetByIdQuery(Guid Id) : IRequest<OperationResult<BrandDTO>>;
}
