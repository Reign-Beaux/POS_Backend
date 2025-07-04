﻿using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using AutoMapper;
using Domain.Entities.Brands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Brands.UseCases.Commands.Create
{
    public sealed class BrandCreateHandler(
        ILoggingMessagesService<BrandCreateHandler> logginMessagesService,
        IMapper mapper,
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

                var brand = mapper.Map<Brand>(request);

                posDb.BrandRepository.Add(brand);
                await posDb.SaveChangesAsync(cancellationToken);

                await logginMessagesService.Handle(BrandCachedKeys.Created, LogLevel.Information);

                return OperationResult.CreatedAtAction(brand.Id);
            }
            catch (Exception ex)
            {
                string message = await logginMessagesService.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, ex);
                return OperationResult.InternalServerError(message);
            }
        }
    }
}
