namespace ElgatoLightControl.ApiClient.Queries;

using Services;

using ValueObjects;

internal class LightStateQueryHandler : IQueryHandler<LightStateQuery, LightState>
{
	public async Task<LightState> Handle(LightStateQuery query, CancellationToken cancellationToken)
	{
		try
		{
			var dto = await ApiHttpClient.GetLightStateAsync(query.IpAddress, cancellationToken);

			return LightState.FromDto(dto);
		}
		catch
		{
			return LightState.Empty;
		}
	}
}
