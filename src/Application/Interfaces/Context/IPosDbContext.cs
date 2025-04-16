using Domain.Entities.Articles;
using Domain.Entities.ArticleTypes;
using Domain.Entities.Brands;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Context
{
    public interface IPosDbContext
    {
        DbSet<Article> Articles { get; set; }   
        DbSet<ArticleType> ArticleTypes { get; set; }
        DbSet<Brand> Brands { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
