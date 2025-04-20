using Domain.Entities.Brands;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class BrandRepository(PosDbContext context) : IBrandRepository
    {
        private readonly PosDbContext _context = context ?? throw new ArgumentException(nameof(context));

        public async Task<IEnumerable<Brand>> GetAll()
            => await _context.Brands.Where(brand => !brand.IsDeleted).ToListAsync();

        public async Task<Brand?> GetById(Guid id)
            => await _context.Brands.SingleOrDefaultAsync(brand => brand.Id == id && !brand.IsDeleted);

        public Task<Brand?> GetByName(string name)
            => _context.Brands.SingleOrDefaultAsync(c => c.Name == name);

        public void Add(Brand brand, CancellationToken cancellationToken = default)
            => _context.Brands.Add(brand);

        public void Update(Brand brand, CancellationToken cancellationToken = default)
            => _context.Brands.Update(brand);

        public void Delete(Brand brand, CancellationToken cancellationToken = default)
            => _context.Brands.Remove(brand);
    }
}
