using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.ArticleTypes;
using MediatR;

namespace Application.UseCases.ArticleTypes.Commands.Create
{
    public sealed class ArticleTypeCreateHandler(IPosDbUnitOfWork _posDb) : IRequestHandler<ArticleTypeCreateCommand, OperationResult<Unit>>
    {
        public async Task<OperationResult<Unit>> Handle(ArticleTypeCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                ArticleType articleType = new()
                {
                    Name = request.Name,
                    Description = request.Description
                };
                _posDb.ArticleTypeRepository.Add(articleType, cancellationToken);
                await _posDb.SaveChangesAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
