using Domain.Entities.Translations;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    internal sealed class TranslationRepository(PosUtilsDbContext context) : ITranslationRepository
    {
        private readonly PosUtilsDbContext context = context ?? throw new ArgumentException(nameof(context));

        public async Task<IEnumerable<Translation>> GetTranslationsForCaching(string keyTranslation)
        {
            var translations = await context.Translations
                .Include(t => t.Language)
                .Include(t => t.KeysForTranslation)
                .Where(t => t.KeysForTranslation.Name == keyTranslation && !t.IsDeleted)
                .ToListAsync();

            return translations;
        }
    }
}
