namespace ElgatoLightControl.ApiClient.Services;

using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Zeroconf;

public static class DeviceDiscovery
{
	private static readonly string ServiceName = "_elg._tcp";

	private static readonly Subject<Unit> DiscoverSubject = new();

	private static readonly Dictionary<string, IPAddress> Devices = new();

	public static EventHandler<DeviceDiscoveryEventArgs> DeviceDiscovered = delegate { };

	static DeviceDiscovery()
	{
		DiscoverSubject
			.Throttle(TimeSpan.FromSeconds(5))
			.SelectMany(_ => Observable.FromAsync(SendQuery))
			.Subscribe();
	}

	public static void Discover() => DiscoverSubject.OnNext(Unit.Default);

	private static void AddDevice(string id, string ip)
	{
		if (IPAddress.TryParse(ip, out var ipAddress) && Devices.TryAdd(id, ipAddress))
		{
			DeviceDiscovered?.Invoke(null, new DeviceDiscoveryEventArgs(id, ipAddress));
		}
	}

	private static async Task SendQuery()
	{
		var hosts = await ZeroconfResolver.ResolveAsync($"{ServiceName}.local.");

		foreach (var host in hosts)
		{
			AddDevice(host.Id, host.IPAddress);
		}
	}
}

public class DeviceDiscoveryEventArgs(string deviceId, IPAddress ipAddress) : EventArgs
{
	public readonly string DeviceId = deviceId;
	public readonly IPAddress IpAddress = ipAddress;
}
