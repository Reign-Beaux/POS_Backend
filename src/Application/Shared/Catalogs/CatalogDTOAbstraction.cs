namespace Application.Shared.Catalogs
{
    public abstract class CatalogDTOAbstraction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
