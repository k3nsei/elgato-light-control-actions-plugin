namespace ElgatoLightControl.ApiClient.Commands
{
    internal record SetBrightnessCommand(String LightIpAddress, Byte Brightness) : ICommand;
}