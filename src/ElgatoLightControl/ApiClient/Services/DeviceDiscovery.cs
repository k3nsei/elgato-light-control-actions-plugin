namespace ElgatoLightControl.ApiClient.Services;

using System.Reactive.Linq;

using Zeroconf;

public class DeviceDiscovery : IDisposable
{
	private static readonly string ServiceName = "_elg._tcp.local.";

	private IDisposable? _subscription;

	public void Dispose()
	{
		this.Stop();

		GC.SuppressFinalize(this);
	}

	public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered = delegate { };

	public void Start() => this.Start(TimeSpan.FromMinutes(5));

	public void Start(TimeSpan interval) =>
		this._subscription = Observable.Timer(TimeSpan.Zero, interval)
			.SelectMany(_ => Observable.FromAsync(this.SendQuery))
			.Subscribe();

	public void Stop() => this._subscription?.Dispose();

	private async Task SendQuery(CancellationToken cancellationToken)
	{
		Logger.Verbose($"Sending query for {ServiceName}");

		await ZeroconfResolver.ResolveAsync(
			ServiceName,
			callback: host =>
			{
				Logger.Verbose($"Discovered device: {host.DisplayName} at {host.IPAddress}");

				this.DeviceDiscovered?.Invoke(this, new DeviceDiscoveredEventArgs(host.DisplayName, host.IPAddress));
			},
			cancellationToken: cancellationToken
		);
	}
}

public class DeviceDiscoveredEventArgs(string deviceId, string ipAddress) : EventArgs
{
	public readonly string DeviceId = deviceId;
	public readonly string IpAddress = ipAddress;
}
