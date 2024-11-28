namespace ElgatoLightApiClient.ValueObjects
{
    public readonly struct PowerState
    {
        public Byte Value { get; init; }

        public PowerState(Byte? value)
        {
            if (value is not (null or 0 or 1))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Power state must be 0 (disabled) or 1 (enabled)");
            }

            this.Value = value ?? 0;
        }

        public PowerState(Boolean? enabled)
        {
            this.Value = enabled == true ? (Byte)1 : (Byte)0;
        }

        public Boolean IsEnabled => this.Value == 1;

        public override String ToString() => this.IsEnabled ? "Enabled" : "Disabled";
    }
}