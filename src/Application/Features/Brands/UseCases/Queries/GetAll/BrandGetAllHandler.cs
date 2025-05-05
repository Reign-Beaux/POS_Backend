using Application.Features.Brands.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Application.Shared;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Brands.UseCases.Queries.GetAll
{
    public sealed class BrandGetAllHandler(
        ILoggingMessagesService<BrandGetAllHandler> loggingMessagesService,
        IMapper mapper,
        IPosDbUnitOfWork posDb) : IRequestHandler<BrandGetAllQuery, OperationResult<IEnumerable<BrandDTO>>>
    {
        public async Task<OperationResult<IEnumerable<BrandDTO>>> Handle(BrandGetAllQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var brands = await posDb.BrandRepository.GetAll();
                if (!brands.Any())
                {
                    string message = await loggingMessagesService.Handle(SharedCachedKeys.RecordsNotFound, LogLevel.Warning);
                    return OperationResult.NotFound(message);
                }

                var brandDTOs = mapper.Map<IEnumerable<BrandDTO>>(brands);
                await loggingMessagesService.Handle(SharedCachedKeys.QuerySuccess, LogLevel.Information);

                return OperationResult.Success(brandDTOs);
            }
            catch (Exception ex)
            {
                string message = await loggingMessagesService.HandleExceptionMessage(SharedCachedKeys.ErrorGettingAll, ex);
                return OperationResult.InternalServerError(message);
            }
        }
    }
}
