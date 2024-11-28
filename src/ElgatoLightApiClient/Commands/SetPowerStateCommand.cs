namespace ElgatoLightApiClient.Commands
{
    internal record SetPowerStateCommand(String LightIpAddress, Boolean Enable) : ICommand;
}