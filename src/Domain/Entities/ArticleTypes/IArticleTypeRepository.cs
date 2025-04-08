namespace Domain.Entities.ArticleTypes
{
    public interface IArticleTypeRepository
    {
        Task<IEnumerable<ArticleType>> GetAll();
        Task<ArticleType?> GetById(Guid id);
        Task<ArticleType?> GetByName(string name);
        void Add(ArticleType article, CancellationToken cancellationToken = default);
        void Update(ArticleType article, CancellationToken cancellationToken = default);
        void Delete(ArticleType article, CancellationToken cancellationToken = default);
    }
}
