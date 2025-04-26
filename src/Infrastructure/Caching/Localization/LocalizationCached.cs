using Application.Interfaces.Caching;
using Application.Interfaces.UnitOfWorks;
using Domain.Entities.Translations;
using Enyim.Caching;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Infrastructure.Caching.Localization
{
    internal sealed class LocalizationCached(
        ILogger<LocalizationCached> logger,
        IMemcachedClient memcachedClient,
        IPosUtilsDbUnitOfWork posUtilsDb) : ILocalizationCached
    {
        private const string CacheKeyPrefix = "Localization";

        public async Task<string> GetText(string keyTranslation)
        {
            try
            {
                var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                var cacheKey = $"{CacheKeyPrefix}_{keyTranslation}_{culture}";

                var dataCached = await memcachedClient.GetValueAsync<string>(cacheKey);

                if (dataCached is null)
                {
                    IEnumerable<Translation> translations = await posUtilsDb.TranslationRepository.GetTranslationsForCaching(keyTranslation);
                    foreach (var translation in translations)
                    {
                        var newCacheKey = $"{CacheKeyPrefix}_{keyTranslation}_{translation.Language.Name}";
                        await memcachedClient.SetAsync(newCacheKey, translation.Text, TimeSpan.FromHours(1));
                        if (culture == translation.Language.Name)
                            dataCached = translation.Text;
                    }
                }

                return dataCached ?? "Translation not available";

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting localization with key {Key}", keyTranslation);
                return "Translation not available";
            }
        }
    }
}
