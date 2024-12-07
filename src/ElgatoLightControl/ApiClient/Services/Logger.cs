namespace ElgatoLightControl.ApiClient.Services
{
    using Shared;

    internal static class Logger
    {
        private static ILogger _logger;

        internal static void Connect(ILogger logger) => _logger = logger;

        internal static void Verbose(String text) => _logger?.Verbose(text);

        internal static void Verbose(Exception ex, String text) => _logger?.Verbose(ex, text);

        internal static void Info(String text) => _logger?.Info(text);

        internal static void Info(Exception ex, String text) => _logger?.Info(ex, text);

        internal static void Warning(String text) => _logger?.Warning(text);

        internal static void Warning(Exception ex, String text) => _logger?.Warning(ex, text);

        internal static void Error(String text) => _logger?.Error(text);

        internal static void Error(Exception ex, String text) => _logger?.Error(ex, text);
    }
}