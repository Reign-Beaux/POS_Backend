using Application.Shared.Catalogs;

namespace Application.DTOs.Articles
{
    public sealed class ArticleDTO : CatalogDTOAbstraction
    {
        public string ArticleTypeName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public decimal Stock { get; set; }
        public decimal MinStockLevel { get; set; }
        public decimal MaxStockLevel { get; set; }
        public string BarCode { get; set; } = string.Empty;
    }
}
