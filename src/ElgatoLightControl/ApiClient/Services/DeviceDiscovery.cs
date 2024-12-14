namespace ElgatoLightControl.ApiClient.Services;

using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Makaretu.Dns;

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

	private static void AddDevice(string deviceId, IPAddress ipAddress)
	{
		Devices.TryAdd(deviceId, ipAddress);

		DeviceDiscovered?.Invoke(null, new DeviceDiscoveryEventArgs(deviceId, ipAddress));
	}

	private static async Task SendQuery()
	{
		using MulticastService mdns = new();
		using ServiceDiscovery sd = new(mdns);
		using CancellationTokenSource cts = new(2000);

		mdns.NetworkInterfaceDiscovered += OnNetworkInterfaceDiscovered;
		sd.ServiceInstanceDiscovered += OnServiceInstanceDiscovered;

		try
		{
			mdns.Start();
			await Task.Delay(2000, cts.Token).ConfigureAwait(false);
		}
		catch (TaskCanceledException)
		{
			// Task was canceled after 2 seconds
		}
		finally
		{
			mdns.Stop();
		}

		return;

		void OnNetworkInterfaceDiscovered(object sender, NetworkInterfaceEventArgs e)
		{
			sd.QueryUnicastServiceInstances(ServiceName);
		}
	}

	private static void OnServiceInstanceDiscovered(object sender, ServiceInstanceDiscoveryEventArgs e)
	{
		var deviceId = e.ServiceInstanceName.ToString();

		if (!deviceId.EndsWith($"{ServiceName}.local"))
		{
			return;
		}

		AddDevice(deviceId, e.RemoteEndPoint.Address);
	}
}

public class DeviceDiscoveryEventArgs(string deviceId, IPAddress ipAddress) : EventArgs
{
	public readonly string DeviceId = deviceId;
	public readonly IPAddress IpAddress = ipAddress;
}
