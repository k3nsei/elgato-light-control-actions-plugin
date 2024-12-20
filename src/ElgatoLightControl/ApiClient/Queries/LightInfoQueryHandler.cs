namespace ElgatoLightControl.ApiClient.Queries;

using Services;

using ValueObjects;

internal class LightInfoQueryHandler : IQueryHandler<LightInfoQuery, LightInfo>
{
	public async Task<LightInfo> Handle(LightInfoQuery query, CancellationToken cancellationToken)
	{
		try
		{
			var dto = await ApiHttpClient.GetLightInfoAsync(query.IpAddress, cancellationToken);

			return LightInfo.FromDto(dto);
		}
		catch
		{
			return LightInfo.Empty;
		}
	}
}
