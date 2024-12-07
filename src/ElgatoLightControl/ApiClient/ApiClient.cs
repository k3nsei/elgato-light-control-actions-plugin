namespace ElgatoLightControl.ApiClient
{
    using Commands;

    using Queries;

    using Services;

    using Shared;

    using ValueObjects;

    public static class ApiClient
    {
        public static void Init(ILogger logger)
        {
            Logger.Connect(logger);

            Dispatcher.RegisterHandler<LightStateQuery>(new LightStateQueryHandler());
            Dispatcher.RegisterHandler<SetPowerStateCommand>(new SetPowerStateCommandHandler());
            Dispatcher.RegisterHandler<SetBrightnessCommand>(new SetBrightnessCommandHandler());
            Dispatcher.RegisterHandler<SetColorTemperatureCommand>(new SetColorTemperatureCommandHandler());
        }

        public static Task<LightState> GetState(String lightIpAddress) =>
            Dispatcher.Query<LightStateQuery, LightState>(new LightStateQuery(lightIpAddress));

        public static Task TurnOn(String lightIpAddress) =>
            Dispatcher.Send(new SetPowerStateCommand(lightIpAddress, true));

        public static Task TurnOff(String lightIpAddress) =>
            Dispatcher.Send(new SetPowerStateCommand(lightIpAddress, false));

        public static Task SetPowerState(String lightIpAddress, Boolean enable) =>
            Dispatcher.Send(new SetPowerStateCommand(lightIpAddress, enable));

        public static Task SetBrightness(String lightIpAddress, Byte brightness) =>
            Dispatcher.Send(new SetBrightnessCommand(lightIpAddress, brightness));

        public static Task SetColorTemperature(String lightIpAddress, UInt16 colorTemperature) =>
            Dispatcher.Send(new SetColorTemperatureCommand(lightIpAddress, colorTemperature));
    }
}