namespace ElgatoLightApiClient.Commands
{
    using Services;

    internal class SetColorTemperatureCommandHandler : ICommandHandler<SetColorTemperatureCommand>
    {
        public Task Handle(SetColorTemperatureCommand command, CancellationToken cancellationToken)
        {
            Logger.Verbose(String.Format(
                "Setting color temperature to {0} for light at {1}",
                command.ColorTemperature,
                command.LightIpAddress
            ));

            return ApiHttpClient.Instance.SetColorTemperatureAsync(
                command.LightIpAddress,
                command.ColorTemperature,
                cancellationToken
            );
        }
    }
}