namespace ElgatoLightControl.ApiClient.ValueObjects;

public readonly struct PowerState
{
	public byte Value { get; init; }

	public PowerState(byte? value)
	{
		if (value is not (null or 0 or 1))
		{
			throw new ArgumentOutOfRangeException(nameof(value), "Power state must be 0 (disabled) or 1 (enabled)");
		}

		this.Value = value ?? 0;
	}

	public PowerState(bool? enabled) => this.Value = enabled == true ? (byte)1 : (byte)0;

	public bool IsEnabled => this.Value == 1;

	public override string ToString() => this.IsEnabled ? "Enabled" : "Disabled";
}
