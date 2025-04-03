namespace Domain.Entities.ArticleTypes
{
    public interface IArticleTypeRepository
    {
        Task<IEnumerable<ArticleType>> GetAll();
        Task<ArticleType?> GetById(Guid id);
        Task<ArticleType?> GetByName(string name);
        void Add(ArticleType article);
        void Update(ArticleType article);
        void Delete(ArticleType article);
    }
}
