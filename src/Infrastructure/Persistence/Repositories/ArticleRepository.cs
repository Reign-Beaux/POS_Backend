using Domain.Entities.Articles;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class ArticleRepository(PosDbContext context) : IArticleRepository
    {
        private readonly PosDbContext _context = context ?? throw new ArgumentException(nameof(context));

        public async Task<IEnumerable<Article>> GetAll()
            => await _context.Articles
                .Include(a => a.ArticleType)
                .Include(a => a.Brand)
                .ToListAsync();

        public Task<Article?> GetById(Guid id)
            => _context.Articles.SingleOrDefaultAsync(c => c.Id == id);

        public void Add(Article article, CancellationToken cancellationToken = default)
            => _context.Articles.Add(article);

        public void Update(Article article, CancellationToken cancellationToken = default)
            => _context.Articles.Update(article);

        public void Delete(Article article, CancellationToken cancellationToken = default)
            => _context.Articles.Remove(article);
    }
}
