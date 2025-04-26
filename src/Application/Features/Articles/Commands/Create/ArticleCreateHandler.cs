using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using AutoMapper;
using Domain.Entities.Articles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Articles.Commands.Create
{
    public sealed class ArticleCreateHandler(
        IMapper mapper,
        IPosDbUnitOfWork posDb,
        ILogger<ArticleCreateHandler> logger) : IRequestHandler<ArticleCreateCommand, OperationResult<Unit>>
    {
        public async Task<OperationResult<Unit>> Handle(ArticleCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var newArticle = mapper.Map<Article>(request);
                posDb.ArticleRepository.Add(newArticle, cancellationToken);
                await posDb.SaveChangesAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating article");
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
