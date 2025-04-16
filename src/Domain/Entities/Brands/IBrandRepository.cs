namespace Domain.Entities.Brands
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Brand>> GetAll();
        Task<Brand?> GetById(Guid id);
        Task<Brand?> GetByName(string name);
        void Add(Brand brand, CancellationToken cancellationToken = default);
        void Update(Brand brand, CancellationToken cancellationToken = default);
        void Delete(Brand brand, CancellationToken cancellationToken = default);
    }
}
