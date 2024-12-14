namespace Loupedeck.ElgatoLightControlPlugin.Helpers;

using ElgatoLightControl.Shared;

internal static class PluginLogger
{
	internal static ILogger Instance { get; private set; }

	internal static void Init(PluginLogFile pluginLogFile) => Instance ??= new Logger(pluginLogFile);

	public static void Verbose(string text) => Instance?.Verbose(text);

	public static void Verbose(Exception ex, string text) => Instance?.Verbose(ex, text);

	public static void Info(string text) => Instance?.Info(text);

	public static void Info(Exception ex, string text) => Instance?.Info(ex, text);

	public static void Warning(string text) => Instance?.Warning(text);

	public static void Warning(Exception ex, string text) => Instance?.Warning(ex, text);

	public static void Error(string text) => Instance?.Error(text);

	public static void Error(Exception ex, string text) => Instance?.Error(ex, text);

	private class Logger : ILogger
	{
		private readonly PluginLogFile _pluginLogFile;

		public Logger(PluginLogFile pluginLogFile)
		{
			pluginLogFile.CheckNullArgument(nameof(pluginLogFile));
			this._pluginLogFile = pluginLogFile;
		}

		void ILogger.Verbose(string text) => this._pluginLogFile?.Verbose(text);

		void ILogger.Verbose(Exception ex, string text) => this._pluginLogFile?.Verbose(ex, text);

		void ILogger.Info(string text) => this._pluginLogFile?.Info(text);

		void ILogger.Info(Exception ex, string text) => this._pluginLogFile?.Info(ex, text);

		void ILogger.Warning(string text) => this._pluginLogFile?.Warning(text);

		void ILogger.Warning(Exception ex, string text) => this._pluginLogFile?.Warning(ex, text);

		void ILogger.Error(string text) => this._pluginLogFile?.Error(text);

		void ILogger.Error(Exception ex, string text) => this._pluginLogFile?.Error(ex, text);
	}
}
