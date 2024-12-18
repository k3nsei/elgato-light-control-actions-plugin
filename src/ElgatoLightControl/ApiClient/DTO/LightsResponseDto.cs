namespace ElgatoLightControl.ApiClient.DTO;

using System.Text.Json.Serialization;

internal record LightsResponseDto(
	[property: JsonPropertyName("numberOfLights")]
	byte NumberOfLights = 0,
	[property: JsonPropertyName("lights")] IReadOnlyList<LightStateDto> Lights = null
);
