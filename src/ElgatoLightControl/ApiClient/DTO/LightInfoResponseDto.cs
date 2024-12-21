namespace ElgatoLightControl.ApiClient.DTO;

using System.Text.Json.Serialization;

using Helpers.Serialization;

public record LightInfoResponseDto(
	[property: JsonPropertyName("productName")]
	string? ProductName = null,
	[property: JsonPropertyName("displayName")]
	string? DisplayName = null,
	[property: JsonPropertyName("serialNumber")]
	string? SerialNumber = null,
	[property: JsonPropertyName("macAddress")]
	string? MacAddress = null,
	[property: JsonPropertyName("hardwareRevision"), JsonConverter(typeof(StringOrNumberConverter))]
	string? HardwareRevision = null,
	[property: JsonPropertyName("hardwareBoardType")]
	ushort? HardwareBoardType = null,
	[property: JsonPropertyName("firmwareVersion"), JsonConverter(typeof(StringOrNumberConverter))]
	string? FirmwareVersion = null,
	[property: JsonPropertyName("firmwareBuildNumber")]
	ushort? FirmwareBuildNumber = null,
	[property: JsonPropertyName("features")]
	IReadOnlyList<string>? Features = null
);
