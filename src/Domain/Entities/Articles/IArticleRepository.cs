namespace Domain.Entities.Articles
{
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetAll();
        Task<Article?> GetById(Guid id);
        void Add(Article article, CancellationToken cancellationToken = default);
        void Update(Article article, CancellationToken cancellationToken = default);
        void Delete(Article article, CancellationToken cancellationToken = default);
    }
}
