using Application.DTOs.ArticleTypes;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using AutoMapper;
using Domain.Entities.ArticleTypes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.ArticleTypes.Queries.GetById
{
    public sealed class ArticleTypeGetByIdHandler(
        ILogger<ArticleTypeGetByIdHandler> logger,
        IMapper mapper,
        IPosDbUnitOfWork posDb) : IRequestHandler<ArticleTypeGetByIdQuery, OperationResult<ArticleTypeDTO>>
    {
        public async Task<OperationResult<ArticleTypeDTO>> Handle(ArticleTypeGetByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                ArticleType? articleType = await posDb.ArticleTypeRepository.GetById(request.Id);
                if (articleType == null)
                {
                    logger.LogWarning("Article type with ID {Id} not found", request.Id);
                    return OperationResult.NotFound($"Article type with ID {request.Id} not found.");
                }

                var response = mapper.Map<ArticleTypeDTO>(articleType);
                return OperationResult.Success(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving article type with ID {Id}", request.Id);
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
