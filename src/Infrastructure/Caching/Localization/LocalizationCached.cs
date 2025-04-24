using Application.Interfaces.Caching;
using Application.Interfaces.UnitOfWorks;
using Enyim.Caching;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Globalization;

namespace Infrastructure.Caching.Localization
{
    internal sealed class LocalizationCached(
        ILogger<LocalizationCached> logger,
        IMemcachedClient memcachedClient,
        IPosUtilsDbUnitOfWork posUtilsDb) : ILocalizationCached
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
        private const string CacheKeyPrefix = "Localization";

        public async Task<string> GetText(string keyTranslation)
        {
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var cacheKey = $"{CacheKeyPrefix}_{keyTranslation}_{culture}";

            var dataCached = await memcachedClient.GetValueAsync<string>(cacheKey);
            if (dataCached is not null)
                return dataCached;

            var keyLock = _locks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

            await keyLock.WaitAsync(); // esperar a que se libere el acceso exclusivo

            try
            {
                // revalidamos cache tras esperar (por si otro lo resolvió mientras tanto)
                dataCached = await memcachedClient.GetValueAsync<string>(cacheKey);
                if (dataCached is not null)
                    return dataCached;

                // obtenemos de base de datos y seteamos en cache
                var translations = await posUtilsDb.TranslationRepository.GetTranslationsForCaching(keyTranslation);
                foreach (var translation in translations)
                {
                    var newCacheKey = $"{CacheKeyPrefix}_{keyTranslation}_{translation.Language.Name}";
                    await memcachedClient.SetAsync(newCacheKey, translation.Text, TimeSpan.FromHours(1));
                    if (culture == translation.Language.Name)
                        dataCached = translation.Text;
                }

                return dataCached ?? string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting localization with key {Key}", keyTranslation);
                return string.Empty;
            }
            finally
            {
                keyLock.Release();
                _locks.TryRemove(cacheKey, out _); // limpiar si ya no es necesario
            }
        }
    }
}
