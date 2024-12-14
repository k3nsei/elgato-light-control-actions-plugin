namespace ElgatoLightControl.ApiClient.ValueObjects;

public readonly struct Brightness(byte? value)
{
	public byte Value { get; init; } = Math.Clamp(value ?? 1, (byte)0, (byte)100);

	public override string ToString() => $"{this.Value}%";
}
