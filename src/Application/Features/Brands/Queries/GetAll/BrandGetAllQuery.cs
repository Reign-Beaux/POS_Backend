using Application.Features.Brands.DTOs;
using Application.OperationResults;
using MediatR;

namespace Application.Features.Brands.Queries.GetAll
{
    public record BrandGetAllQuery() : IRequest<OperationResult<IEnumerable<BrandDTO>>>;
}
