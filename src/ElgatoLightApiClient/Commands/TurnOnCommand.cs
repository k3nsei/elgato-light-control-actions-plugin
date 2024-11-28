namespace ElgatoLightApiClient.Commands
{
    internal record TurnOnCommand(String LightIpAddress) : ICommand;
}