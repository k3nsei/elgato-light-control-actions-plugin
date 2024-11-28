namespace ElgatoLightApiClient.Commands
{
    internal record SetColorTemperatureCommand(String LightIpAddress, UInt16 ColorTemperature) : ICommand;
}