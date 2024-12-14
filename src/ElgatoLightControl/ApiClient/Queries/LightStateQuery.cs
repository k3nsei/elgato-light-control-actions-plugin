namespace ElgatoLightControl.ApiClient.Queries;

using ValueObjects;

internal record LightStateQuery(string LightIpAddress) : IQuery<LightState>;
