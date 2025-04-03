namespace Domain.Entities
{
    public abstract class BaseCatalogs : BaseEntities
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
