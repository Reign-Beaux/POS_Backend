using Application.Interfaces.Context;
using Application.Interfaces.UnitOfWorks;
using Domain.Entities.ArticleTypes;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.UnitOfWorks
{
    public class PosDbUnitOfWork(
        IPosDbContext context,
        IArticleTypeRepository articleTypeRepository) : IPosDbUnitOfWork
    {
        private readonly IPosDbContext _context = context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        public IArticleTypeRepository ArticleTypeRepository { get; } = articleTypeRepository;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            if (_context is PosDbContext dbContext)
            {
                _transaction = await dbContext.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }

                    if (_context is PosDbContext dbContext)
                    {
                        dbContext.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        ~PosDbUnitOfWork()
        {
            Dispose(false);
        }
    }
}
