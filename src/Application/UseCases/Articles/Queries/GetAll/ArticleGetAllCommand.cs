using Application.DTOs.Articles;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using AutoMapper;
using Domain.Entities.Articles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Articles.Queries.GetAll
{
    public sealed class ArticleGetAllCommand(
        IMapper mapper,
        IPosDbUnitOfWork posDb,
        ILogger<ArticleGetAllCommand> logger) : IRequestHandler<ArticleGetAllQuery, OperationResult<IEnumerable<ArticleDTO>>>
    {
        public async Task<OperationResult<IEnumerable<ArticleDTO>>> Handle(ArticleGetAllQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Article> articles = await posDb.ArticleRepository.GetAll();
                var articleDTOs = mapper.Map<IEnumerable<ArticleDTO>>(articles);
                return OperationResult.Success(articleDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting articles.");
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
