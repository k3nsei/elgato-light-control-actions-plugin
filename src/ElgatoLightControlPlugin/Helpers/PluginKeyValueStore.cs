namespace Loupedeck.ElgatoLightControlPlugin.Helpers;

using System.Text;

public static class PluginKeyValueStore
{
	public delegate bool TryGetDelegate(string key, out string value);

	private static TryGetDelegate _tryGet;

	private static Action<string, string> _set;

	private static Action<string> _remove;

	public static void Init(
		TryGetDelegate readAction,
		Action<string, string> writeAction,
		Action<string> deleteAction
	)
	{
		_tryGet = readAction;
		_set = writeAction;
		_remove = deleteAction;
	}

	public static bool TryGet(string key, out string decodedValue)
	{
		if (!_tryGet(key, out var value))
		{
			decodedValue = null;
			return false;
		}

		decodedValue = Encoding.UTF8.GetString(Convert.FromBase64String(value));
		return true;
	}

	public static string Get(string key) => TryGet(key, out var value) ? value : string.Empty;

	public static void Set(string key, string value) => _set(key, Convert.ToBase64String(
		Encoding.UTF8.GetBytes(value)
	));

	public static void Remove(string key) => _remove(key);
}
