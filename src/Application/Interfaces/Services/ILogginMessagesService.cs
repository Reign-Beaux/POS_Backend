using Microsoft.Extensions.Logging;

namespace Application.Interfaces.Services
{
    public interface ILogginMessagesService
    {
        Task<string> Handle(string localizationKey, string name, LogLevel logLevel);
    }
}
