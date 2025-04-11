namespace Domain.Entities.Articles
{
    public class Article : BaseCatalogs
    {
        public Guid ArticleTypeId { get; set; }
        public Guid BrandId { get; set; }
        public decimal Stock { get; set; }
        public decimal MinStockLevel { get; set; }
        public decimal MaxStockLevel { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
    }
}
