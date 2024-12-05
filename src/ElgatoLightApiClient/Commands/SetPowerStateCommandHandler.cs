namespace ElgatoLightApiClient.Commands
{
    using Services;

    internal class SetPowerStateCommandHandler : ICommandHandler<SetPowerStateCommand>
    {
        public Task Handle(SetPowerStateCommand command, CancellationToken cancellationToken)
        {
            Logger.Verbose(String.Format(
                "Setting power state to {0} for light at {1}",
                command.Enable,
                command.LightIpAddress
            ));

            return ApiHttpClient.Instance.SetPowerStateAsync(
                command.LightIpAddress,
                command.Enable,
                cancellationToken
            );
        }
    }
}