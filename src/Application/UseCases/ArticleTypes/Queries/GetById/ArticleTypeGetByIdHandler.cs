using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.ArticleTypes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.ArticleTypes.Queries.GetById
{
    public sealed class ArticleTypeGetByIdHandler(
        ILogger<ArticleTypeGetByIdHandler> logger,
        IPosDbUnitOfWork posDb) : IRequestHandler<ArticleTypeGetByIdQuery, OperationResult<ArticleType>>
    {
        private readonly ILogger<ArticleTypeGetByIdHandler> _logger = logger;
        private readonly IPosDbUnitOfWork _posDb = posDb;

        public async Task<OperationResult<ArticleType>> Handle(ArticleTypeGetByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                ArticleType? articleType = await _posDb.ArticleTypeRepository.GetById(request.Id);
                if (articleType == null)
                {
                    _logger.LogWarning("Article type with ID {Id} not found", request.Id);
                    return OperationResult.NotFound($"Article type with ID {request.Id} not found.");
                }

                return OperationResult.Success(articleType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving article type with ID {Id}", request.Id);
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
