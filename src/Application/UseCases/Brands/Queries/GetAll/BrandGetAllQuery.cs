using Application.DTOs.Brands;
using Application.OperationResults;
using MediatR;

namespace Application.UseCases.Brands.Queries.GetAll
{
    public record BrandGetAllQuery() : IRequest<OperationResult<IEnumerable<BrandDTO>>>;
}
