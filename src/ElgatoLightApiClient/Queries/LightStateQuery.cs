namespace ElgatoLightApiClient.Queries
{
    using ValueObjects;

    internal record LightStateQuery(String LightIpAddress) : IQuery<LightState>;
}