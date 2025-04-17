using Domain.Entities.Articles;
using Domain.Entities.ArticleTypes;
using Domain.Entities.Brands;

namespace Application.Interfaces.UnitOfWorks
{
    public interface IPosDbUnitOfWork : IDisposable
    {
        IArticleRepository ArticleRepository { get; }
        IArticleTypeRepository ArticleTypeRepository { get; }
        IBrandRepository BrandRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync();
    }
}
