namespace ElgatoLightControl.ApiClient.ValueObjects;

using Helpers;

public readonly struct ColorTemperature
{
	public ushort Value { get; init; }

	public ushort Kelvin { get; init; }

	public ColorTemperature(ushort? value)
	{
		this.Value = Math.Clamp(value ?? 143, (ushort)143, (ushort)344);
		this.Kelvin = TemperatureConverter.MiredsToKelvin(this.Value);
	}

	public override string ToString() => $"{this.Value} mireds";

	// public override String ToString() => $"{this.Kelvin.ToString()}K";
}
