namespace ElgatoLightControl.ApiClient.DTO
{
    using System.Text.Json.Serialization;

    internal record SetColorTemperatureRequestDto(
        [property: JsonPropertyName("lights")] IReadOnlyList<LightColorTemperatureStateDto> Lights = null
    );
}