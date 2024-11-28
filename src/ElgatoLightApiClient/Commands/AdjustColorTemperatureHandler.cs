namespace ElgatoLightApiClient.Commands
{
    using Services;

    internal class SetColorTemperatureCommandHandler : ICommandHandler<SetColorTemperatureCommand>
    {
        public async Task Handle(SetColorTemperatureCommand command, CancellationToken cancellationToken) =>
            await ApiHttpClient.Instance.SetColorTemperatureAsync(command.LightIpAddress, command.ColorTemperature, cancellationToken);
    }
}