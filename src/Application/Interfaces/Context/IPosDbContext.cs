using Domain.Entities.ArticleTypes;
using Domain.Entities.Brands;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Context
{
    public interface IPosDbContext
    {
        DbSet<Brand> Brands { get; set; }
        DbSet<ArticleType> ArticleTypes { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
