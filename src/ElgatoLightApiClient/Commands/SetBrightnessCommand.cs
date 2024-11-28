namespace ElgatoLightApiClient.Commands
{
    internal record SetBrightnessCommand(String LightIpAddress, Byte Brightness) : ICommand;
}