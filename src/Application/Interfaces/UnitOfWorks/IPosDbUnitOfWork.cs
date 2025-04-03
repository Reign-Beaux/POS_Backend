using Domain.Entities.ArticleTypes;

namespace Application.Interfaces.UnitOfWorks
{
    public interface IPosDbUnitOfWork : IDisposable
    {
        IArticleTypeRepository ArticleTypeRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
