using Application.Features.Brands.DTOs;
using Application.OperationResults;
using MediatR;

namespace Application.Features.Brands.Queries.GetByName
{
    public record BrandGetByNameQuery(string Name) : IRequest<OperationResult<BrandDTO>>;
}
