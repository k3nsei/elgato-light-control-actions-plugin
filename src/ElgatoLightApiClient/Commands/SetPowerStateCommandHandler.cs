namespace ElgatoLightApiClient.Commands
{
    using Services;

    internal class SetPowerStateCommandHandler : ICommandHandler<SetPowerStateCommand>
    {
        public async Task Handle(SetPowerStateCommand command, CancellationToken cancellationToken) =>
            await ApiHttpClient.Instance.SetPowerStateAsync(command.LightIpAddress, command.Enable, cancellationToken);
    }
}