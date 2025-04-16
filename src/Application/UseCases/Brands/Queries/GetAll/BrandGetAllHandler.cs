using Application.DTOs.Brands;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using AutoMapper;
using Domain.Entities.Brands;
using MediatR;

namespace Application.UseCases.Brands.Queries.GetAll
{
    public sealed class BrandGetAllHandler(
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
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
