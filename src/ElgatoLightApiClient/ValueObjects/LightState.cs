namespace ElgatoLightApiClient.ValueObjects
{
    using DTO;

    public readonly struct LightState(Byte? powerState, Byte? brightness, UInt16? colorTemperature)
    {
        public required PowerState PowerState { get; init; } = new() { Value = powerState ?? default };
        public required Brightness Brightness { get; init; } = new() { Value = brightness ?? default };
        public required ColorTemperature ColorTemperature { get; init; } = new() { Value = colorTemperature ?? default };

        internal static LightState Empty => new() { PowerState = default, Brightness = default, ColorTemperature = default };

        internal static LightState FromDto(LightStateDto dto)
        {
            return new()
            {
                PowerState = new() { Value = dto.On },
                Brightness = new() { Value = dto.Brightness },
                ColorTemperature = new() { Value = dto.Temperature }
            };
        }

        public override String ToString()
        {
            return $"LightState(Power state: {this.PowerState.ToString()}, " +
                   $"Brightness: {this.Brightness.ToString()}, " +
                   $"Color Temperature: {this.ColorTemperature.ToString()})";
        }
    }
}