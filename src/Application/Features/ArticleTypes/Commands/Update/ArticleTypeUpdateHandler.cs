using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.ArticleTypes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.ArticleTypes.Commands.Update
{
    public sealed class ArticleTypeUpdateHandler(
        IPosDbUnitOfWork posDb,
        ILogger<ArticleTypeUpdateHandler> logger) : IRequestHandler<ArticleTypeUpdateCommand, OperationResult<Unit>>
    {
        public async Task<OperationResult<Unit>> Handle(ArticleTypeUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                ArticleType? articleType = await posDb.ArticleTypeRepository.GetById(request.Id);
                if (articleType == null)
                {
                    logger.LogWarning("Error updating article type with ID {Id}", request.Id);
                    return OperationResult.NotFound("Article type not found.");
                }

                articleType.Name = request.Name;
                articleType.Description = request.Description;
                posDb.ArticleTypeRepository.Update(articleType, cancellationToken);
                await posDb.SaveChangesAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating article type with ID {Id}", request.Id);
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
