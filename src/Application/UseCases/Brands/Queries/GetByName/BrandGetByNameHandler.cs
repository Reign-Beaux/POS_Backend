using Application.DTOs.Brands;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Application.UseCases.Brands.Queries.GetAll;
using AutoMapper;
using Domain.Entities.Brands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Brands.Queries.GetByName
{
    public sealed class BrandGetByNameHandler(
        ILogger<BrandGetAllHandler> logger,
        IMapper mapper,
        IPosDbUnitOfWork posDb) : IRequestHandler<BrandGetByNameQuery, OperationResult<BrandDTO>>
    {
        public async Task<OperationResult<BrandDTO>> Handle(BrandGetByNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Brand? brand = await posDb.BrandRepository.GetByName(request.Name);
                if (brand == null)
                {
                    logger.LogWarning("Brand with name {Name} not found", request.Name);
                    return OperationResult.NotFound($"Brand with name {request.Name} not found.");
                }
                var response = mapper.Map<BrandDTO>(brand);
                return OperationResult.Success(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving brand with ID {Id}", request.Id);
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
