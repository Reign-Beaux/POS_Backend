using Domain.Entities.Articles;

namespace Domain.Entities.ArticleTypes
{
    public class ArticleType : BaseCatalog
    {
        public ICollection<Article> Articles { get; set; } = [];
    }
}
