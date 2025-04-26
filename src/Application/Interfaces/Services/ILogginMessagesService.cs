using Microsoft.Extensions.Logging;

namespace Application.Interfaces.Services
{
    public interface ILogginMessagesService<T>
    {
        Task<string> Handle(string localizationKey, string name, LogLevel logLevel);
        Task<string> Handle(string localizationKey, LogLevel logLevel);
    }
}
