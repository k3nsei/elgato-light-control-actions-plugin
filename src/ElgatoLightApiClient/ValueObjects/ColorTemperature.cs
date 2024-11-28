namespace ElgatoLightApiClient.ValueObjects
{
    using Helpers;

    public readonly struct ColorTemperature
    {
        public required UInt16 Value { get; init; }

        private readonly UInt16 Kelvin;

        public ColorTemperature(UInt16? value)
        {
            var mireds = Math.Clamp(value ?? 143, (UInt16)143, (UInt16)344);

            this.Value = mireds;
            this.Kelvin = TemperatureConverter.MiredsToKelvin(mireds);
        }

        public override String ToString() => $"{this.Value.ToString()} mireds";

        // public override String ToString() => $"{this.Kelvin.ToString()}K";
    }
}