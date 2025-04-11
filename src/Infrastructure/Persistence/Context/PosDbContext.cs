using Application.Interfaces.Context;
using Domain.Entities.ArticleTypes;
using Domain.Entities.Brands;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context
{
    public class PosDbContext(DbContextOptions<PosDbContext> options) : DbContext(options), IPosDbContext
    {
        public DbSet<ArticleType> ArticleTypes { get; set; }
        public DbSet<Brand> Brands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PosDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
