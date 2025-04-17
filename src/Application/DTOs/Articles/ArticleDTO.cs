namespace Application.DTOs.Articles
{
    public record ArticleDTO(
        string ArticleTypeName,
        string BrandName,
        decimal Stock,
        decimal MinStockLevel,
        decimal MaxStockLevel,
        string BarCode) : CatalogDTO;
}
