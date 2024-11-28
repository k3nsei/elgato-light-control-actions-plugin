namespace ElgatoLightApiClient.Commands
{
    using Services;

    internal class TurnOffCommandHandler : ICommandHandler<TurnOffCommand>
    {
        public async Task Handle(TurnOffCommand command, CancellationToken cancellationToken) =>
            await ApiHttpClient.Instance.SetPowerStateAsync(command.LightIpAddress, false, cancellationToken);
    }
}