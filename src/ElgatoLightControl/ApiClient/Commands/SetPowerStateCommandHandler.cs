namespace ElgatoLightControl.ApiClient.Commands;

using Services;

internal class SetPowerStateCommandHandler : ICommandHandler<SetPowerStateCommand>
{
	public Task Handle(SetPowerStateCommand command, CancellationToken cancellationToken)
	{
		Logger.Verbose(string.Format(
			"Setting power state to {0} for light at {1}",
			command.Enable,
			command.LightIpAddress
		));

		return ApiHttpClient.SetPowerStateAsync(
			command.LightIpAddress,
			command.Enable,
			cancellationToken
		);
	}
}
