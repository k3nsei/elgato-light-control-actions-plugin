namespace ElgatoLightControl.ApiClient.Commands
{
    using Services;

    internal class SetBrightnessCommandHandler : ICommandHandler<SetBrightnessCommand>
    {
        public Task Handle(SetBrightnessCommand command, CancellationToken cancellationToken)
        {
            Logger.Verbose(String.Format(
                "Setting brightness to {0} for light at {1}",
                command.Brightness,
                command.LightIpAddress
            ));

            return ApiHttpClient.Instance.SetBrightnessAsync(
                command.LightIpAddress,
                command.Brightness,
                cancellationToken
            );
        }
    }
}