namespace ElgatoLightControl.ApiClient.ValueObjects;

public readonly struct PowerState(byte? value)
{
	public byte Value { get; } = value > 0 ? (byte)1 : (byte)0;

	public bool IsEnabled => this.Value == 1;

	public override string ToString() => this.IsEnabled ? "Enabled" : "Disabled";
}
