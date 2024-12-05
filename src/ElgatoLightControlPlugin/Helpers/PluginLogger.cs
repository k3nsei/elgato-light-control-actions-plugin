namespace Loupedeck.ElgatoLightControlPlugin.Helpers
{
    using System;

    using Shared;

    internal static class PluginLogger
    {
        internal static ILogger Instance { get; private set; }

        internal static void Init(PluginLogFile pluginLogFile) => Instance ??= new Logger(pluginLogFile);

        public static void Verbose(String text) => Instance?.Verbose(text);

        public static void Verbose(Exception ex, String text) => Instance?.Verbose(ex, text);

        public static void Info(String text) => Instance?.Info(text);

        public static void Info(Exception ex, String text) => Instance?.Info(ex, text);

        public static void Warning(String text) => Instance?.Warning(text);

        public static void Warning(Exception ex, String text) => Instance?.Warning(ex, text);

        public static void Error(String text) => Instance?.Error(text);

        public static void Error(Exception ex, String text) => Instance?.Error(ex, text);

        private class Logger : ILogger
        {
            private readonly PluginLogFile _pluginLogFile;

            public Logger(PluginLogFile pluginLogFile)
            {
                pluginLogFile.CheckNullArgument(nameof(pluginLogFile));
                this._pluginLogFile = pluginLogFile;
            }

            void ILogger.Verbose(String text) => this._pluginLogFile?.Verbose(text);

            void ILogger.Verbose(Exception ex, String text) => this._pluginLogFile?.Verbose(ex, text);

            void ILogger.Info(String text) => this._pluginLogFile?.Info(text);

            void ILogger.Info(Exception ex, String text) => this._pluginLogFile?.Info(ex, text);

            void ILogger.Warning(String text) => this._pluginLogFile?.Warning(text);

            void ILogger.Warning(Exception ex, String text) => this._pluginLogFile?.Warning(ex, text);

            void ILogger.Error(String text) => this._pluginLogFile?.Error(text);

            void ILogger.Error(Exception ex, String text) => this._pluginLogFile?.Error(ex, text);
        }
    }
}