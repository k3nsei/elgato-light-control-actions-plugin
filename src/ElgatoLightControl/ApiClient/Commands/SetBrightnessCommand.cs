namespace ElgatoLightControl.ApiClient.Commands;

internal record SetBrightnessCommand(
	string LightIpAddress,
	byte Brightness
) : ICommand;
