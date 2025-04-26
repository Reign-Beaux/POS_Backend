using Application.Interfaces.Caching;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    internal class LogginMessagesService<T>(
        ILocalizationCached localization,
        ILogger<T> logger) : ILogginMessagesService<T>
    {
        private readonly Dictionary<LogLevel, Action<string>> _logActions = new()
        {
            { LogLevel.Warning, msg => logger.LogWarning(msg) },
            { LogLevel.Information, msg => logger.LogInformation(msg) },
            { LogLevel.Error, msg => logger.LogError(msg) }
        };

        public async Task<string> Handle(string localizationKey, string value, LogLevel logLevel)
        {
            try
            {
                string message = await localization.GetText(localizationKey);
                message = string.Format(message, value);

                LogMessage(logLevel, message);

                return message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener el mensaje de localización.");
                throw;
            }
        }

        public async Task<string> Handle(string localizationKey, LogLevel logLevel)
        {
            try
            {
                string message = await localization.GetText(localizationKey);

                LogMessage(logLevel, message);

                return message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener el mensaje de localización.");
                throw;
            }
        }

        private void LogMessage(LogLevel logLevel, string message)
        {
            if (_logActions.TryGetValue(logLevel, out var logAction))
            {
                logAction(message);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "Nivel de log no válido");
            }
        }
    }
}
