namespace ElgatoLightApiClient.ValueObjects
{
    using DTO;

    public readonly struct LightState(Byte? powerState, Byte? brightness, UInt16? colorTemperature)
    {
        public PowerState PowerState { get; init; } = new(powerState);
        public Brightness Brightness { get; init; } = new(brightness);
        public ColorTemperature ColorTemperature { get; init; } = new(colorTemperature);

        internal static LightState Empty => new();

        internal static LightState FromDto(LightStateDto dto) => new(dto.On, dto.Brightness, dto.Temperature);

        public override String ToString()
        {
            return "LightState( " +
                   $"Power state: {this.PowerState}, " +
                   $"Brightness: {this.Brightness}, " +
                   $"Color Temperature: {this.ColorTemperature} " +
                   ")";
        }
    }
}