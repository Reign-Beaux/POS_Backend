using Application.Features.Brands.DTOs;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using AutoMapper;
using Domain.Entities.Brands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Brands.UseCases.Queries.GetAll
{
    public sealed class BrandGetAllHandler(
        ILogger<BrandGetAllHandler> logger,
        IMapper mapper,
        IPosDbUnitOfWork posDb) : IRequestHandler<BrandGetAllQuery, OperationResult<IEnumerable<BrandDTO>>>
    {
        public async Task<OperationResult<IEnumerable<BrandDTO>>> Handle(BrandGetAllQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Brand> brands = await posDb.BrandRepository.GetAll();
                IEnumerable<BrandDTO> brandDTOs = mapper.Map<IEnumerable<BrandDTO>>(brands);
                return OperationResult.Success(brandDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all brands");
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
