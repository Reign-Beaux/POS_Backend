using Microsoft.Extensions.Logging;

namespace Application.Interfaces.Services
{
    public interface ILoggingMessagesService<T>
    {
        Task<string> Handle(string localizationKey, string name, LogLevel logLevel);
        Task<string> Handle(string localizationKey, LogLevel logLevel);
        Task<string> HandleExceptionMessage(string localizationKey, Exception exception);
    }
}
