namespace ElgatoLightControl.ApiClient;

using System.Reactive.Linq;
using System.Reactive.Subjects;

using Queries;

using Services;

using Shared;

using ValueObjects;

public static class ApiClient
{
	private static readonly Subject<(string IpAddress, bool Enable)> PowerStateSubject = new();

	private static readonly Subject<(string IpAddress, byte Brightness)> BrightnessSubject = new();

	private static readonly Subject<(string IpAddress, ushort ColorTemperature)> ColorTemperatureSubject = new();

	public static void Init(ILogger logger)
	{
		Logger.Connect(logger);

		Dispatcher.RegisterHandler<LightInfoQuery>(new LightInfoQueryHandler());
		Dispatcher.RegisterHandler<LightStateQuery>(new LightStateQueryHandler());

		CreateInvokeCommandObservable(
			PowerStateSubject,
			ApiHttpClient.SetPowerStateAsync,
			TimeSpan.FromMilliseconds(100),
			"Failed to set power state"
		).Subscribe();

		CreateInvokeCommandObservable(
			BrightnessSubject,
			ApiHttpClient.SetBrightnessAsync,
			TimeSpan.FromMilliseconds(50),
			"Failed to set brightness"
		).Subscribe();

		CreateInvokeCommandObservable(
			ColorTemperatureSubject,
			ApiHttpClient.SetColorTemperatureAsync,
			TimeSpan.FromMilliseconds(50),
			"Failed to set color temperature"
		).Subscribe();
	}

	public static Task<LightInfo> GetLightInfo(string ipAddress) =>
		Dispatcher.Query<LightInfoQuery, LightInfo>(new LightInfoQuery(ipAddress));

	public static Task<LightState> GetLightState(string ipAddress) =>
		Dispatcher.Query<LightStateQuery, LightState>(new LightStateQuery(ipAddress));

	public static void TurnOn(string ipAddress) =>
		SetPowerState(ipAddress, true);

	public static void TurnOff(string ipAddress) =>
		SetPowerState(ipAddress, false);

	public static void SetPowerState(string ipAddress, bool enable) =>
		PowerStateSubject.OnNext((ipAddress, enable));

	public static void SetBrightness(string ipAddress, byte brightness) =>
		BrightnessSubject.OnNext((ipAddress, brightness));

	public static void SetColorTemperature(string ipAddress, ushort colorTemperature) =>
		ColorTemperatureSubject.OnNext((ipAddress, colorTemperature));

	private static IObservable<(string IpAddress, T Value, CancellationToken CancellationToken)>
		CreateInvokeCommandObservable<T>(
			ISubject<(string IpAddress, T Value)> subject,
			Func<string, T, CancellationToken, Task> action,
			TimeSpan throttleTime, string errorMessage
		)
	{
		return subject
			.GroupBy(value => value.IpAddress)
			.SelectMany(group =>
				group
					.Throttle(throttleTime)
					.Select(value => (
						value.IpAddress,
						value.Value,
						CancellationTokenSource: new CancellationTokenSource()
					))
					.Scan((prev, curr) =>
					{
						prev.CancellationTokenSource.Cancel();

						return curr;
					})
					.Select(value => (
						value.IpAddress,
						value.Value,
						CancellationToken: value.CancellationTokenSource.Token
					))
					.Do(async void (value) =>
					{
						try
						{
							await action(value.IpAddress, value.Value, value.CancellationToken);
						}
						catch (TaskCanceledException)
						{
							// Task was canceled, no further action needed
						}
						catch (Exception ex)
						{
							Logger.Error(ex, errorMessage);
						}
					})
			);
	}
}
