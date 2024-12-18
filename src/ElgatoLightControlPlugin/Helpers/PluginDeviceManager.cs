namespace Loupedeck.ElgatoLightControlPlugin.Helpers;

using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Text.Json;

using Constants;

using ElgatoLightControl.ApiClient.Services;

using IDeviceCollection = IReadOnlyCollection<(string DeviceId, System.Net.IPAddress IPAddress)>;

public static class PluginDeviceManager
{
	private static readonly Dictionary<string, IPAddress> DevicesDict = new();

	private static readonly Subject<Unit> DeviceDiscoveredSubject = new();

	public static EventHandler<DeviceListChangedEventArgs> DeviceListChanged = delegate { };

	public static IDeviceCollection Devices =>
		DevicesDict.Select(x => (x.Key, x.Value)).ToList().AsReadOnly();

	public static void Init()
	{
		DeviceDiscoveredSubject
			.Throttle(TimeSpan.FromMilliseconds(100))
			.Subscribe(_ =>
			{
				SaveKnownDevices();

				DeviceListChanged.Invoke(null, new DeviceListChangedEventArgs(Devices));
			});
	}

	public static void OnLoad()
	{
		Restore();

		Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(60))
			.Subscribe(_ => DeviceDiscovery.Discover());

		DeviceDiscovery.DeviceDiscovered += (_, e) => AddDevice(e.DeviceId, e.IpAddress);
	}

	public static void OnUnload()
	{
		// Add any necessary cleanup code here
	}

	private static void AddDevice(string deviceId, IPAddress ipAddress)
	{
		DevicesDict.TryAdd(deviceId, ipAddress);
		DeviceDiscoveredSubject.OnNext(Unit.Default);
	}

	private static void Restore()
	{
		foreach (var (deviceId, ipAddress) in ReadKnownDevices())
		{
			AddDevice(deviceId, ipAddress);
		}
	}

	private static List<(string DeviceId, IPAddress IPAddress)> ReadKnownDevices()
	{
		try
		{
			var data = PluginKeyValueStore.Get(SettingName.KnownDevices);

			if (!string.IsNullOrWhiteSpace(data))
			{
				return JsonSerializer.Deserialize<List<List<string>>>(data)
					.Select(x => (x[0], IPAddress.Parse(x[1])))
					.ToList();
			}
		}
		catch (Exception ex)
		{
			var errorMessage = ex switch
			{
				SerializationException => "Serialization error of stored known devices",
				_ => "Unexpected error while reading known devices"
			};

			PluginLogger.Error(ex, errorMessage);
		}

		return [];
	}

	private static void SaveKnownDevices()
	{
		var devices = DevicesDict.Select(
			x => new List<string> { x.Key, x.Value.ToString() }
		);

		var value = JsonSerializer.Serialize(devices);

		PluginKeyValueStore.Set(SettingName.KnownDevices, value);
	}
}

public class DeviceListChangedEventArgs(IDeviceCollection devices) : EventArgs
{
	public readonly IDeviceCollection Devices = devices;
}
