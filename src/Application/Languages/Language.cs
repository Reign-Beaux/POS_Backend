using System.Resources;

namespace Application.Languages
{
    public static class Language
    {
        public static readonly string[] SupportedLanguages = ["en", "es"];
        public const string DefaultLanguage = "en";

        private static readonly ResourceManager _resourceManager;

        static Language()
        {
            _resourceManager = new ResourceManager("Application.Languages.CatalogMessages", typeof(Language).Assembly);
        }

        public static string? GetString(string name)
        {
            return _resourceManager.GetString(name);
        }
    }
}
