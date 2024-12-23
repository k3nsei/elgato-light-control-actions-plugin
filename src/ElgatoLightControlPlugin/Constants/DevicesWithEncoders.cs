namespace Loupedeck.ElgatoLightControlPlugin.Constants;

public static class DevicesWithEncoders
{
	public static readonly DeviceType Supported = DeviceType.Loupedeck20 | DeviceType.Loupedeck30 | DeviceType.Loupedeck40;
	public static readonly DeviceType NotSupported = DeviceType.All & ~Supported;
}
