namespace ElgatoLightControl.ApiClient.Commands;

internal record SetPowerStateCommand(string LightIpAddress, bool Enable) : ICommand;
