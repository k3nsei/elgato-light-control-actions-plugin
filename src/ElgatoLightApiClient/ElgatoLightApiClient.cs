namespace ElgatoLightApiClient
{
    using System.Collections.Immutable;

    using Commands;

    using Queries;

    using Services;

    using ValueObjects;

    public static class ElgatoLightApiClient
    {
        public static IObservable<IImmutableList<String>> Devices => DeviceDiscovery.Instance.Devices;

        public static void Init(Action<String> logInfo, Action<Exception> logError)
        {
            DeviceDiscovery.Instance.logInfo = logInfo;
            DeviceDiscovery.Instance.logError = logError;

            Dispatcher.RegisterHandler<LightStateQuery>(new LightStateQueryHandler());
            Dispatcher.RegisterHandler<TurnOnCommand>(new TurnOnCommandHandler());
            Dispatcher.RegisterHandler<TurnOffCommand>(new TurnOffCommandHandler());
            Dispatcher.RegisterHandler<SetBrightnessCommand>(new SetBrightnessCommandHandler());
            Dispatcher.RegisterHandler<SetColorTemperatureCommand>(new SetColorTemperatureCommandHandler());
        }

        public static Task<LightState> GetState(String lightIpAddress) =>
            Dispatcher.Query<LightStateQuery, LightState>(new LightStateQuery(lightIpAddress));

        public static Task TurnOn(String lightIpAddress) =>
            Dispatcher.Send(new TurnOnCommand(lightIpAddress));

        public static Task TurnOff(String lightIpAddress) =>
            Dispatcher.Send(new TurnOffCommand(lightIpAddress));

        public static Task SetBrightness(String lightIpAddress, Byte brightness) =>
            Dispatcher.Send(new SetBrightnessCommand(lightIpAddress, brightness));

        public static Task SetColorTemperature(String lightIpAddress, UInt16 colorTemperature) =>
            Dispatcher.Send(new SetColorTemperatureCommand(lightIpAddress, colorTemperature));
    }
}