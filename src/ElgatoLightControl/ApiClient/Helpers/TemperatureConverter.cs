namespace ElgatoLightControl.ApiClient.Helpers;

public static class TemperatureConverter
{
	public static ushort MiredToKelvin(ushort value) => (ushort)(Math.Round(1000000d / value / 50d) * 50);

	public static ushort KelvinToMired(ushort value) => (ushort)(1000000d / value);
}
