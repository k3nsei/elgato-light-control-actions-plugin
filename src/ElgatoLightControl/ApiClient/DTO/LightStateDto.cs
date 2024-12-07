namespace ElgatoLightControl.ApiClient.DTO
{
    using System.Text.Json.Serialization;

    internal record LightStateDto(
        // Power state: 0 = Disabled, 1 = Enabled
        [property: JsonPropertyName("on")] Byte On = 0,
        // Brightness value in percent (0-100)
        [property: JsonPropertyName("brightness")]
        Byte Brightness = 1,
        // Temperature value in mireds (143-344). For more details, see https://en.wikipedia.org/wiki/Mired
        [property: JsonPropertyName("temperature")]
        UInt16 Temperature = 143
    );

    internal record LightPowerStateDto(
        [property: JsonPropertyName("on")] Byte On = 0
    );

    internal record LightBrightnessStateDto(
        [property: JsonPropertyName("brightness")]
        Byte Brightness = 1
    );

    internal record LightColorTemperatureStateDto(
        [property: JsonPropertyName("temperature")]
        UInt16 Temperature = 143
    );
}