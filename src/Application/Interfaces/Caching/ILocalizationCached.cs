namespace Application.Interfaces.Caching
{
    public interface ILocalizationCached
    {
        Task<string> GetText(string keyTranslation);
    }
}
