namespace ElgatoLightControl.ApiClient.Queries;

using ValueObjects;

internal record LightInfoQuery(string IpAddress) : IQuery<LightInfo>;
