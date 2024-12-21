namespace ElgatoLightControl.ApiClient.ValueObjects;

using DTO;

using TLightState = (
	bool PowerState,
	byte Brightness,
	ushort ColorTemperature
	);

public readonly struct LightState(byte? powerState, byte? brightness, ushort? colorTemperature)
{
	private PowerState PowerState { get; } = new(powerState);

	private Brightness Brightness { get; } = new(brightness);

	private ColorTemperature ColorTemperature { get; } = new(colorTemperature);

	public static LightState Empty => new(0, 25, 143);

	internal static LightState FromDto(LightStateDto dto) =>
		new(dto.On, dto.Brightness, dto.Temperature);

	public TLightState Value =>
	(
		this.PowerState.IsEnabled,
		this.Brightness.Value,
		this.ColorTemperature.Value
	);

	public override string ToString() =>
		"LightState( " +
		$"Power state: {this.PowerState}, " +
		$"Brightness: {this.Brightness}, " +
		$"Color Temperature: {this.ColorTemperature} " +
		")";
}
