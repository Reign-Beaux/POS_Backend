using System.Collections.Concurrent;
using System.Resources;

namespace Application.Languages
{
    public class Language
    {
        private ConcurrentDictionary<ResourcesTypes, ResourceManager> _resourceManagers = new();

        public string? GetString(ResourcesTypes resource, string name)
        {
            if (!_resourceManagers.TryGetValue(resource, out var resourceManager))
            {
                resourceManager = new ResourceManager($"Application.Languages.{resource}", typeof(Language).Assembly);
                _resourceManagers[resource] = resourceManager;
            }

            return resourceManager.GetString(name);
        }
    }
}
