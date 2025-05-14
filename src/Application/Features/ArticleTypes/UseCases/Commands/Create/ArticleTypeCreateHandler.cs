using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using AutoMapper;
using Domain.Entities.ArticleTypes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.ArticleTypes.UseCases.Commands.Create
{
    public class ArticleTypeCreateHandler(
        ILoggingMessagesService<ArticleTypeCreateHandler> loggingMessagesService,
        IMapper mapper,
        IPosDbUnitOfWork posDB) : IRequestHandler<ArticleTypeCreateCommand, OperationResult<Guid>>
    {
        public async Task<OperationResult<Guid>> Handle(ArticleTypeCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await posDB.ArticleTypeRepository.GetByName(request.Name);
                if (exists is not null)
                {
                    string message = await loggingMessagesService.Handle(ArticleTypeCachedKeys.AlreadyExists, LogLevel.Warning);
                    return OperationResult.Conflict(message);
                }

                var brand = mapper.Map<ArticleType>(request);

                posDB.ArticleTypeRepository.Add(brand);
                await posDB.SaveChangesAsync(cancellationToken);

                await loggingMessagesService.Handle(ArticleTypeCachedKeys.Created, LogLevel.Information);

                return OperationResult.CreatedAtAction(brand.Id);
            }
            catch (Exception ex)
            {
                string message = await loggingMessagesService.HandleExceptionMessage(ArticleTypeCachedKeys.ErrorCreating, ex);
                return OperationResult.InternalServerError(message);
            }
        }
    }
}
