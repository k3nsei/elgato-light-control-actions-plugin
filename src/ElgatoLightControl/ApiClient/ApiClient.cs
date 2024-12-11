namespace ElgatoLightControl.ApiClient
{
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Queries;

    using Services;

    using Shared;

    using ValueObjects;

    public static class ApiClient
    {
        private static readonly Subject<(String IpAddress, Boolean Enable)> PowerStateSubject = new();

        private static readonly Subject<(String IpAddress, Byte Brightness)> BrightnessSubject = new();

        private static readonly Subject<(String IpAddress, UInt16 ColorTemperature)> ColorTemperatureSubject = new();

        public static void Init(ILogger logger)
        {
            Logger.Connect(logger);

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

        public static Task<LightState> GetState(String lightIpAddress) =>
            Dispatcher.Query<LightStateQuery, LightState>(new LightStateQuery(lightIpAddress));

        public static void TurnOn(String lightIpAddress) =>
            SetPowerState(lightIpAddress, true);

        public static void TurnOff(String lightIpAddress) =>
            SetPowerState(lightIpAddress, false);

        public static void SetPowerState(String lightIpAddress, Boolean enable) =>
            PowerStateSubject.OnNext((lightIpAddress, enable));

        public static void SetBrightness(String ipAddress, Byte brightness) =>
            ColorTemperatureSubject.OnNext((ipAddress, brightness));

        public static void SetColorTemperature(String ipAddress, UInt16 colorTemperature) =>
            ColorTemperatureSubject.OnNext((ipAddress, colorTemperature));

        private static IObservable<(String IpAddress, T Value, CancellationToken CancellationToken)>
            CreateInvokeCommandObservable<T>(
                ISubject<(String IpAddress, T Value)> subject,
                Func<String, T, CancellationToken, Task> action,
                TimeSpan throttleTime, String errorMessage
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
                            catch (Exception ex)
                            {
                                Logger.Error(ex, errorMessage);
                            }
                        })
                );
        }
    }
}