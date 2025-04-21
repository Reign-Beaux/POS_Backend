namespace Domain.Entities
{
    public abstract class BaseCatalog : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public static int MaxNameLength => 64;
        public static int MaxDescriptionLength => 256;
    }
}
