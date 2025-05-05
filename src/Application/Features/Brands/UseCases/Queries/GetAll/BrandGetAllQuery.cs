using Application.Features.Brands.DTOs;
using Application.OperationResults;
using MediatR;

namespace Application.Features.Brands.UseCases.Queries.GetAll
{
    public record class BrandGetAllQuery : IRequest<OperationResult<IEnumerable<BrandDTO>>>;
}
