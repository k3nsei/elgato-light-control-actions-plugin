namespace ElgatoLightControl.ApiClient.ValueObjects;

using DTO;

public readonly struct LightState(byte? powerState, byte? brightness, ushort? colorTemperature)
{
	public PowerState PowerState { get; init; } = new(powerState);
	public Brightness Brightness { get; init; } = new(brightness);
	public ColorTemperature ColorTemperature { get; init; } = new(colorTemperature);

	internal static LightState Empty => new();

	internal static LightState FromDto(LightStateDto dto) => new(dto.On, dto.Brightness, dto.Temperature);

	public override string ToString()
	{
		return "LightState( " +
		       $"Power state: {this.PowerState}, " +
		       $"Brightness: {this.Brightness}, " +
		       $"Color Temperature: {this.ColorTemperature} " +
		       ")";
	}
}
