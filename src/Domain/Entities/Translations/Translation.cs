using Domain.Entities.KeysForTranslations;
using Domain.Entities.Languages;

namespace Domain.Entities.Translations;

public partial class Translation
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public Guid KeysForTranslationId { get; set; }

    public Guid LanguageId { get; set; }

    public string Text { get; set; } = null!;

    public virtual KeysForTranslation KeysForTranslation { get; set; } = null!;

    public virtual Language Language { get; set; } = null!;
}
