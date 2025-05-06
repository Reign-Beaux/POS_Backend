using Application.Features.Brands.DTOs;
using Application.Features.Brands.UseCases.Queries.GetAll;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Application.Shared;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Brands.UseCases.Queries.GetById
{
    public sealed class BrandGetByIdHandler(
        ILoggingMessagesService<BrandGetAllHandler> loggingMessagesService,
        IMapper mapper,
        IPosDbUnitOfWork posDb) : IRequestHandler<BrandGetByIdQuery, OperationResult<BrandDTO>>
    {
        public async Task<OperationResult<BrandDTO>> Handle(BrandGetByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var brand = await posDb.BrandRepository.GetById(request.Id);
                if (brand is null)
                {
                    string message = await loggingMessagesService.Handle(BrandCachedKeys.NotFound, LogLevel.Warning);
                    return OperationResult.NotFound(message);
                }

                var brandDTO = mapper.Map<BrandDTO>(brand);

                await loggingMessagesService.Handle(SharedCachedKeys.QuerySuccess, LogLevel.Information);
                return OperationResult.Success(brandDTO);
            }
            catch (Exception ex)
            {
                string message = await loggingMessagesService.HandleExceptionMessage(BrandCachedKeys.NotFound, ex);
                return OperationResult.InternalServerError(message);
            }
        }
    }
}
