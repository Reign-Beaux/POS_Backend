using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.Brands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Brands.Commands.Create
{
    public sealed class BrandCreateHandler(
        ILogger<BrandCreateHandler> logger,
        IPosDbUnitOfWork posDb) : IRequestHandler<BrandCreateCommand, OperationResult<Unit>>
    {
        public async Task<OperationResult<Unit>> Handle(BrandCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await posDb.BrandRepository.GetByName(request.Name);
                if (exists is not null)
                {
                    logger.LogWarning("Brand with name {Name} already exists", request.Name);
                    return OperationResult.BadRequest($"Brand with name {request.Name} already exists.");
                }

                Brand brand = new()
                {
                    Name = request.Name,
                    Description = request.Description
                };
                posDb.BrandRepository.Add(brand, cancellationToken);
                await posDb.SaveChangesAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating brand with name {Name}", request.Name);
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
