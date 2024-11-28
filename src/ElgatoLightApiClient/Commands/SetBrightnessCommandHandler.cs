namespace ElgatoLightApiClient.Commands
{
    using Services;

    internal class SetBrightnessCommandHandler : ICommandHandler<SetBrightnessCommand>
    {
        public async Task Handle(SetBrightnessCommand command, CancellationToken cancellationToken) =>
            await ApiHttpClient.Instance.SetBrightnessAsync(command.LightIpAddress, command.Brightness, cancellationToken);
    }
}