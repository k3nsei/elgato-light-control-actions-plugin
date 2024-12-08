namespace ElgatoLightControl.ApiClient.Queries
{
    using Services;

    using ValueObjects;

    internal class LightStateQueryHandler : IQueryHandler<LightStateQuery, LightState>
    {
        public async Task<LightState> Handle(LightStateQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var result = await ApiHttpClient.GetStateAsync(query.LightIpAddress, cancellationToken);

                return LightState.FromDto(result);
            }
            catch
            {
                return LightState.Empty;
            }
        }
    }
}