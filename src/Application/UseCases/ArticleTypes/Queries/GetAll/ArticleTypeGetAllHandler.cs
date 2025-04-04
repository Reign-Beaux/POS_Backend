using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.ArticleTypes;
using MediatR;

namespace Application.UseCases.ArticleTypes.Queries.GetAll
{
    public sealed class ArticleTypeGetAllHandler(IPosDbUnitOfWork posDb) : IRequestHandler<ArticleTypeGetAllQuery, OperationResult<IEnumerable<ArticleType>>>
    {
        private readonly IPosDbUnitOfWork _posDb = posDb;

        public async Task<OperationResult<IEnumerable<ArticleType>>> Handle(ArticleTypeGetAllQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<ArticleType> articleTypes = await _posDb.ArticleTypeRepository.GetAll();
                return OperationResult.Success(articleTypes);
            }
            catch (Exception ex)
            {
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
