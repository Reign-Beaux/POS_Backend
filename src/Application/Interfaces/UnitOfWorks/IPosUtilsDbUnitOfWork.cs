using Domain.Entities.Translations;

namespace Application.Interfaces.UnitOfWorks
{
    public interface IPosUtilsDbUnitOfWork : IDisposable
    {
        ITranslationRepository TranslationRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync();
    }
}
