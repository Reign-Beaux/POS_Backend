using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.Brands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Brands.UseCases.Commands.Create
{
    public sealed class BrandCreateHandler(
        ILogginMessagesService<BrandCreateHandler> logginMessagesService,
        IPosDbUnitOfWork posDb) : IRequestHandler<BrandCreateCommand, OperationResult<Guid>>
    {
        public async Task<OperationResult<Guid>> Handle(BrandCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await posDb.BrandRepository.GetByName(request.Name);
                if (exists is not null)
                {
                    string message = await logginMessagesService.Handle(BrandCachedKeys.AlreadyExists, request.Name, LogLevel.Warning);
                    return OperationResult.Conflict(message);
                }

                Brand brand = new()
                {
                    Name = request.Name,
                    Description = request.Description
                };

                posDb.BrandRepository.Add(brand, cancellationToken);
                await posDb.SaveChangesAsync(cancellationToken);

                await logginMessagesService.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information);

                return OperationResult.CreatedAtAction(brand.Id);
            }
            catch (Exception ex)
            {
                string message = await logginMessagesService.Handle(BrandCachedKeys.ErrorCreating, request.Name, LogLevel.Error);
                return OperationResult.InternalServerError(message);
            }
        }
    }
}
