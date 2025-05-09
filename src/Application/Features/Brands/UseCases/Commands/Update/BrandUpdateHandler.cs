using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Brands.UseCases.Commands.Update
{
    public sealed class BrandUpdateHandler(
        ILoggingMessagesService<BrandUpdateHandler> logginMessagesService,
        IMapper mapper,
        IPosDbUnitOfWork posDb) : IRequestHandler<BrandUpdateCommand, OperationResult<Unit>>
    {
        public async Task<OperationResult<Unit>> Handle(BrandUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var brand = await posDb.BrandRepository.GetById(request.Id!.Value);
                if (brand is null)
                {
                    string message = await logginMessagesService.Handle(BrandCachedKeys.NotFound, request.Name, LogLevel.Warning);
                    return OperationResult.NotFound(message);
                }

                mapper.Map(request, brand);

                posDb.BrandRepository.Update(brand!);

                await posDb.SaveChangesAsync(cancellationToken);
                await logginMessagesService.Handle(BrandCachedKeys.Updated, request.Name, LogLevel.Information);    
                return OperationResult.NoContent();
            }
            catch (Exception ex)
            {
                string message = await logginMessagesService.HandleExceptionMessage(BrandCachedKeys.ErrorUpdating, ex);
                return OperationResult.InternalServerError(message);
            }
        }
    }
}
