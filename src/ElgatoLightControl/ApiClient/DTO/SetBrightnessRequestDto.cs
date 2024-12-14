namespace ElgatoLightControl.ApiClient.DTO;

using System.Text.Json.Serialization;

internal record SetBrightnessRequestDto(
	[property: JsonPropertyName("lights")] IReadOnlyList<LightBrightnessStateDto> Lights = null
);
