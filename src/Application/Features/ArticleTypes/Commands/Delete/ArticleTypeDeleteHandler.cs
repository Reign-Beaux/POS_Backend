using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.ArticleTypes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.ArticleTypes.Commands.Delete
{
    public sealed class ArticleTypeDeleteHandler(
        IPosDbUnitOfWork pos,
        ILogger<ArticleTypeDeleteHandler> logger) : IRequestHandler<ArticleTypeDeleteCommand, OperationResult<Unit>>
    {
        public async Task<OperationResult<Unit>> Handle(ArticleTypeDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                ArticleType? articleType = await pos.ArticleTypeRepository.GetById(request.Id);
                if (articleType == null)
                {
                    logger.LogWarning("Error deleting article type with ID {Id}", request.Id);
                    return OperationResult.NotFound("Article type not found.");
                }

                pos.ArticleTypeRepository.Delete(articleType, cancellationToken);
                await pos.SaveChangesAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting article type with ID {Id}", request.Id);
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
