namespace Domain.Entities.Translations
{
    public interface ITranslationRepository
    {
        Task<IEnumerable<Translation>> GetTranslationsForCaching(string keyTranslation);
    }
}
