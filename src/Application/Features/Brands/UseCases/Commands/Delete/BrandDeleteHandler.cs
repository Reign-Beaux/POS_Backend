using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Brands.UseCases.Commands.Delete
{
    public sealed class BrandDeleteHandler(
        ILoggingMessagesService<BrandDeleteHandler> logginMessagesService,
        IPosDbUnitOfWork posDb) : IRequestHandler<BrandDeleteCommand, OperationResult<Unit>>
    {
        public async Task<OperationResult<Unit>> Handle(BrandDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var brand = await posDb.BrandRepository.GetById(request.Id);
                if (brand is null)
                {
                    string message = await logginMessagesService.Handle(BrandCachedKeys.NotFound, request.Id.ToString(), LogLevel.Warning);
                    return OperationResult.NotFound(message);
                }

                posDb.BrandRepository.Delete(brand);

                await posDb.SaveChangesAsync(cancellationToken);
                await logginMessagesService.Handle(BrandCachedKeys.Deleted, brand.Name, LogLevel.Information);

                return OperationResult.NoContent();
            }
            catch (Exception ex)
            {
                string message = await logginMessagesService.HandleExceptionMessage(BrandCachedKeys.ErrorDeleting, ex);
                return OperationResult.InternalServerError(message);
            }
    }
}
