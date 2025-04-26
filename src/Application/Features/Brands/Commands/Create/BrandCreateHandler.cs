using Application.Interfaces.Caching;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.Brands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Brands.Commands.Create
{
    public sealed class BrandCreateHandler(
        ILocalizationCached localization,
        ILogger<BrandCreateHandler> logger,
        IPosDbUnitOfWork posDb) : IRequestHandler<BrandCreateCommand, OperationResult<string>>
    {
        public async Task<OperationResult<string>> Handle(BrandCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await posDb.BrandRepository.GetByName(request.Name);
                if (exists is not null)
                {
                    string alreadyExistsMessage = await localization.GetText(BrandCachedKeys.AlreadyExists);
                    alreadyExistsMessage = string.Format(alreadyExistsMessage, request.Name);
                    logger.LogWarning(alreadyExistsMessage);
                    return OperationResult.BadRequest(alreadyExistsMessage);
                }

                Brand brand = new()
                {
                    Name = request.Name,
                    Description = request.Description
                };
                posDb.BrandRepository.Add(brand, cancellationToken);
                await posDb.SaveChangesAsync(cancellationToken);

                string createdSuccessfullyMessage = await localization.GetText(BrandCachedKeys.CreatedSuccessfully);
                createdSuccessfullyMessage = string.Format(createdSuccessfullyMessage, request.Name);
                logger.LogInformation(createdSuccessfullyMessage);

                return OperationResult.Success(createdSuccessfullyMessage);
            }
            catch (Exception ex)
            {
                string errorCreatingMessage = await localization.GetText(BrandCachedKeys.ErrorCreating);
                errorCreatingMessage = string.Format(errorCreatingMessage, request.Name);
                logger.LogError(ex, errorCreatingMessage);
                return OperationResult.InternalServerError(errorCreatingMessage);
            }
        }
    }
}
