namespace ElgatoLightControl.ApiClient.Queries;

using ValueObjects;

internal record LightStateQuery(string IpAddress) : IQuery<LightState>;
