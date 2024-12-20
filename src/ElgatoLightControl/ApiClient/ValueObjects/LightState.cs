namespace ElgatoLightControl.ApiClient.ValueObjects;

using DTO;

public readonly struct LightState(byte? powerState, byte? brightness, ushort? colorTemperature)
{
	private PowerState PowerState { get; } = new(powerState);

	private Brightness Brightness { get; } = new(brightness);

	private ColorTemperature ColorTemperature { get; } = new(colorTemperature);

	internal static LightState Empty => new();

	internal static LightState FromDto(LightStateDto dto) =>
		new(dto.On, dto.Brightness, dto.Temperature);

	public (PowerState PowerState, Brightness Brightness, ColorTemperature ColorTemperature) Value =>
		(this.PowerState, this.Brightness, this.ColorTemperature);

	public override string ToString() =>
		"LightState( " +
		$"Power state: {this.PowerState}, " +
		$"Brightness: {this.Brightness}, " +
		$"Color Temperature: {this.ColorTemperature} " +
		")";
}
