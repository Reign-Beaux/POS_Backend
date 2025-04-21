using Domain.Entities.Articles;

namespace Domain.Entities.Brands
{
    public class Brand : BaseCatalog
    {
        public ICollection<Article> Articles { get; set; } = [];
    }
}
