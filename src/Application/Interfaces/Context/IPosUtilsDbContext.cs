using Domain.Entities.KeysForTranslations;
using Domain.Entities.Languages;
using Domain.Entities.Translations;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Context
{
    public interface IPosUtilsDbContext
    {
        DbSet<KeysForTranslation> KeysForTranslations { get; set; }
        DbSet<Language> Languages { get; set; }
        DbSet<Translation> Translations { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
