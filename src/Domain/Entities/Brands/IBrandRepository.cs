namespace Domain.Entities.Brands
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Brand>> GetAll();
        Task<Brand?> GetById(Guid id);
        Task<Brand?> GetByName(string name);
        void Add(Brand brand);
        void Update(Brand brand);
        void Delete(Brand brand);
    }
}
