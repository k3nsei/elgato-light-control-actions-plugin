namespace ElgatoLightControl.ApiClient.Commands;

internal record SetColorTemperatureCommand(string LightIpAddress, ushort ColorTemperature) : ICommand;
