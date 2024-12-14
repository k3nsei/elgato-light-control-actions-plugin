namespace ElgatoLightControl.ApiClient.Services;

using Shared;

internal static class Logger
{
	private static ILogger _logger;

	internal static void Connect(ILogger logger) => _logger = logger;

	internal static void Verbose(string text) => _logger?.Verbose(text);

	internal static void Verbose(Exception ex, string text) => _logger?.Verbose(ex, text);

	internal static void Info(string text) => _logger?.Info(text);

	internal static void Info(Exception ex, string text) => _logger?.Info(ex, text);

	internal static void Warning(string text) => _logger?.Warning(text);

	internal static void Warning(Exception ex, string text) => _logger?.Warning(ex, text);

	internal static void Error(string text) => _logger?.Error(text);

	internal static void Error(Exception ex, string text) => _logger?.Error(ex, text);
}
