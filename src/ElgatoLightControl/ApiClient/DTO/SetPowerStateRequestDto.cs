namespace ElgatoLightControl.ApiClient.DTO;

using System.Text.Json.Serialization;

internal record SetPowerStateRequestDto(
	[property: JsonPropertyName("lights")] IReadOnlyList<LightPowerStateDto> Lights = null
);
