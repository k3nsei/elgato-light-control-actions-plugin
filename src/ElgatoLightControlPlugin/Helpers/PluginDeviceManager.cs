namespace Loupedeck.ElgatoLightControlPlugin.Helpers;

using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Text.Json;

using Constants;

using ElgatoLightControl.ApiClient;
using ElgatoLightControl.ApiClient.Services;
using ElgatoLightControl.ApiClient.ValueObjects;

using TDeviceInput = (string DeviceId, System.Net.IPAddress IpAddress);
using TDeviceEntry = (
	string DeviceId,
	System.Net.IPAddress IpAddress,
	ElgatoLightControl.ApiClient.ValueObjects.LightInfo LightInfo,
	ElgatoLightControl.ApiClient.ValueObjects.LightState LightState
	);

public static class PluginDeviceManager
{
	private static readonly DeviceDiscovery DeviceDiscovery = new();

	private static readonly HashSet<string> KnownDeviceIds = [];

	private static readonly Subject<TDeviceInput> DevicesSubject = new();

	public static readonly IObservable<IReadOnlyList<TDeviceEntry>> DevicesObservable = DevicesSubject
		.Where(input => !KnownDeviceIds.Contains(input.DeviceId))
		.Do(input => KnownDeviceIds.Add(input.DeviceId))
		.SelectMany(MapToDeviceEntry)
		.Scan(MergeDeviceEntryLists)
		.Do(SaveKnownDevices)
		.Publish()
		.RefCount();

	public static void Init()
	{
		Restore();

		DeviceDiscovery.DeviceDiscovered += (_, e) => AddDevice(e.DeviceId, e.IpAddress);
	}

	public static void OnLoad() => DeviceDiscovery.Start();

	public static void OnUnload() => DeviceDiscovery.Stop();

	private static void AddDevice(string id, string ip)
	{
		if (IPAddress.TryParse(ip, out var ipAddress))
		{
			AddDevice(id, ipAddress);
		}
	}

	private static void AddDevice(string id, IPAddress ip) => DevicesSubject.OnNext((id, ip));

	private static async Task<List<TDeviceEntry>> MapToDeviceEntry(TDeviceInput input)
	{
		try
		{
			var lightInfo = await ApiClient.GetLightInfo(input.IpAddress.ToString());

			PluginLogger.Verbose($"Light info for {input.DeviceId} at {input.IpAddress}: {lightInfo}");

			return new[] { (input.DeviceId, input.IpAddress, lightInfo, LightState.Empty) }.ToList();
		}
		catch (Exception e)
		{
			PluginLogger.Error(e, $"Failed to get light info for {input.DeviceId} at {input.IpAddress}");
		}

		return new[] { (input.DeviceId, input.IpAddress, LightInfo.Empty, LightState.Empty) }.ToList();
	}

	private static List<TDeviceEntry> MergeDeviceEntryLists(
		List<TDeviceEntry> prev,
		List<TDeviceEntry> curr
	) => prev?.Concat(curr).DistinctBy(entry => entry.DeviceId).ToList() ?? curr;

	private static void Restore() =>
		ReadKnownDevices()
			.ToList()
			.ForEach(
				device => AddDevice(device.DeviceId, device.IpAddress)
			);

	private static List<TDeviceInput> ReadKnownDevices()
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

	private static void SaveKnownDevices(List<TDeviceEntry> devices)
	{
		var data = devices.Select(
			device => new List<string> { device.DeviceId, device.IpAddress.ToString() }
		);

		var content = JsonSerializer.Serialize(data);

		PluginKeyValueStore.Set(SettingName.KnownDevices, content);
	}
}
