using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.Brands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Brands.Commands.Update
{
    public sealed class BrandUpdateHandler(
        IPosDbUnitOfWork posDb,
        ILogger<BrandUpdateHandler> logger) : IRequestHandler<BrandUpdateCommand, OperationResult<Unit>>
    {
        public async Task<OperationResult<Unit>> Handle(BrandUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Brand? brand = await posDb.BrandRepository.GetById(request.Id);
                if (brand == null)
                {
                    logger.LogWarning("Error updating brand with ID {Id}", request.Id);
                    return OperationResult.NotFound("Brand not found.");
                }

                brand.Name = request.Name;
                brand.Description = request.Description;
                posDb.BrandRepository.Update(brand, cancellationToken);
                await posDb.SaveChangesAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating brand with ID {Id}", request.Id);
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
