namespace ElgatoLightControl.Shared
{
    public interface ILogger
    {
        void Verbose(string text);
        void Verbose(Exception ex, string text);

        void Info(string text);
        void Info(Exception ex, string text);

        void Warning(string text);
        void Warning(Exception ex, string text);

        void Error(string text);
        void Error(Exception ex, string text);
    }
}