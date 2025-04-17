using Application.DTOs.Brands;
using Application.OperationResults;
using MediatR;

namespace Application.UseCases.Brands.Queries.GetByName
{
    public record BrandGetByNameQuery(string Name) : IRequest<OperationResult<BrandDTO>>;
}
