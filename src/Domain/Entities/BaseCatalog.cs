namespace Domain.Entities
{
    public abstract class BaseCatalog : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public const int MaxNameLength = 64;
        public const int MaxDescriptionLength = 256;
    }
}
