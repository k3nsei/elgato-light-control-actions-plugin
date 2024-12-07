namespace ElgatoLightControl.ApiClient.Commands
{
    internal record SetPowerStateCommand(String LightIpAddress, Boolean Enable) : ICommand;
}