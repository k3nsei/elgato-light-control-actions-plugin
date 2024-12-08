namespace Loupedeck.ElgatoLightControlPlugin.Helpers
{
    using System.Text;

    public static class PluginKeyValueStore
    {
        public delegate Boolean TryGetDelegate(String key, out String value);

        private static TryGetDelegate _tryGet;

        private static Action<String, String> _set;

        private static Action<String> _remove;

        public static void Init(
            TryGetDelegate readAction,
            Action<String, String> writeAction,
            Action<String> deleteAction
        )
        {
            _tryGet = readAction;
            _set = writeAction;
            _remove = deleteAction;
        }

        public static Boolean TryGet(String key, out String decodedValue)
        {
            if (!_tryGet(key, out var value))
            {
                decodedValue = null;
                return false;
            }

            decodedValue = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            return true;
        }

        public static String Get(String key) => TryGet(key, out var value) ? value : String.Empty;

        public static void Set(String key, String value) => _set(key, Convert.ToBase64String(
            Encoding.UTF8.GetBytes(value)
        ));

        public static void Remove(String key) => _remove(key);
    }
}