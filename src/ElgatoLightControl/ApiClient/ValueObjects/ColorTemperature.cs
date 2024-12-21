namespace ElgatoLightControl.ApiClient.ValueObjects;

using Helpers;

public readonly struct ColorTemperature(ushort? value)
{
	public ushort Value { get; } = Math.Clamp(value ?? 143, (ushort)143, (ushort)344);

	private ushort Kelvin => TemperatureConverter.MiredToKelvin(this.Value);

	public override string ToString() => $"{this.Kelvin}K";
}
