using Application.DTOs.Brands;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using AutoMapper;
using Domain.Entities.Brands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Brands.Queries.GetById
{
    public sealed class BrandGetByIdHandler(
        ILogger<BrandGetByIdHandler> logger,
        IMapper mapper,
        IPosDbUnitOfWork posDb) : IRequestHandler<BrandGetByIdQuery, OperationResult<BrandDTO>>
    {
        public async Task<OperationResult<BrandDTO>> Handle(BrandGetByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Brand? brand = await posDb.BrandRepository.GetById(request.Id);
                if (brand == null)
                {
                    logger.LogWarning("Brand with ID {Id} not found", request.Id);
                    return OperationResult.NotFound($"Brand with ID {request.Id} not found.");
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
