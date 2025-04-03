using Domain.Entities.ArticleTypes;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Context
{
    public interface IPosDbContext
    {
        DbSet<ArticleType> ArticleTypes { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
