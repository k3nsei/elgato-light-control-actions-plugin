namespace ElgatoLightApiClient.DTO
{
    using System.Text.Json.Serialization;

    internal record LightsResponseDto(
        [property: JsonPropertyName("numberOfLights")]
        Byte NumberOfLights = 0,
        [property: JsonPropertyName("lights")] IReadOnlyList<LightStateDto> Lights = null
    );
}