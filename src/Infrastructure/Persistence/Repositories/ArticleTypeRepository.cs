﻿using Domain.Entities.ArticleTypes;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ArticleTypeRepository(PosDbContext context) : IArticleTypeRepository
    {
        private readonly PosDbContext _context = context ?? throw new ArgumentException(nameof(context));

        public async Task<IEnumerable<ArticleType>> GetAll()
            => await _context.ArticleTypes.ToListAsync();

        public async Task<ArticleType?> GetById(Guid id)
            => await _context.ArticleTypes.SingleOrDefaultAsync(c => c.Id == id);

        public Task<ArticleType?> GetByName(string name)
            => _context.ArticleTypes.SingleOrDefaultAsync(c => c.Name == name);

        public void Add(ArticleType article)
            => _context.ArticleTypes.Add(article);

        public void Update(ArticleType article)
            => _context.ArticleTypes.Update(article);

        public void Delete(ArticleType article)
            => _context.ArticleTypes.Remove(article);
    }
}
