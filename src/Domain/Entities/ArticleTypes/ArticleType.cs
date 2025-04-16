using Domain.Entities.Articles;

namespace Domain.Entities.ArticleTypes
{
    public class ArticleType : BaseCatalogs
    {
        public ICollection<Article> Articles { get; set; } = [];
    }
}
