using Application.DTOs.Brands;
using Application.OperationResults;
using MediatR;

namespace Application.UseCases.Brands.Queries.GetById
{
    public record BrandGetByIdQuery(Guid Id) : IRequest<OperationResult<BrandDTO>>;
}
