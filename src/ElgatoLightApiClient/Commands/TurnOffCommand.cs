namespace ElgatoLightApiClient.Commands
{
    internal record TurnOffCommand(String LightIpAddress) : ICommand;
}