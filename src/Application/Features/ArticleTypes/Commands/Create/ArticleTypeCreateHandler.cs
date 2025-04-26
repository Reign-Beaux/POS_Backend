using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.ArticleTypes;
using MediatR;

namespace Application.Features.ArticleTypes.Commands.Create
{
    public sealed class ArticleTypeCreateHandler(IPosDbUnitOfWork posDb) : IRequestHandler<ArticleTypeCreateCommand, OperationResult<Unit>>
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
                posDb.ArticleTypeRepository.Add(articleType, cancellationToken);
                await posDb.SaveChangesAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
