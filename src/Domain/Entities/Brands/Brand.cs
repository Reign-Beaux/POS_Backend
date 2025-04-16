using Domain.Entities.Articles;

namespace Domain.Entities.Brands
{
    public class Brand : BaseCatalogs
    {
        public ICollection<Article> Articles { get; set; } = [];
    }
}
