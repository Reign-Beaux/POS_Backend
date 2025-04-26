using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.Brands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Brands.UseCases.Commands.Delete
{
    public sealed class BrandDeleteHandler(
        ILogger<BrandDeleteHandler> logger,
        IPosDbUnitOfWork pos) : IRequestHandler<BrandDeleteCommand, OperationResult<Unit>>
    {
        public async Task<OperationResult<Unit>> Handle(BrandDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Brand? brand = await pos.BrandRepository.GetById(request.Id);
                if (brand == null)
                {
                    logger.LogWarning("Error deleting brand with ID {Id}", request.Id);
                    return OperationResult.NotFound("Brand not found.");
                }

                pos.BrandRepository.Delete(brand, cancellationToken);
                await pos.SaveChangesAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting brand with ID {Id}", request.Id);
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
