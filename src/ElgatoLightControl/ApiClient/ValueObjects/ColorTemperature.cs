namespace ElgatoLightControl.ApiClient.ValueObjects
{
    using Helpers;

    public readonly struct ColorTemperature
    {
        public UInt16 Value { get; init; }

        public UInt16 Kelvin { get; init; }

        public ColorTemperature(UInt16? value)
        {
            this.Value = Math.Clamp(value ?? 143, (UInt16)143, (UInt16)344);
            this.Kelvin = TemperatureConverter.MiredsToKelvin(this.Value);
        }

        public override String ToString() => $"{this.Value} mireds";

        // public override String ToString() => $"{this.Kelvin.ToString()}K";
    }
}