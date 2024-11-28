namespace ElgatoLightApiClient.Commands
{
    using Services;

    internal class TurnOnCommandHandler : ICommandHandler<TurnOnCommand>
    {
        public async Task Handle(TurnOnCommand command, CancellationToken cancellationToken) =>
            await ApiHttpClient.Instance.SetPowerStateAsync(command.LightIpAddress, true, cancellationToken);
    }
}