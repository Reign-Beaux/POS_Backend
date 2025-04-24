using Domain.Entities.Translations;

namespace Domain.Entities.Languages;

public partial class Language
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Translation> Translations { get; set; } = [];
}
