namespace ElgatoLightControl.ApiClient.DTO;

using System.Text.Json.Serialization;

public record LightInfoResponseDto(
	[property: JsonPropertyName("productName")]
	string ProductName,
	[property: JsonPropertyName("displayName")]
	string DisplayName,
	[property: JsonPropertyName("serialNumber")]
	string SerialNumber,
	[property: JsonPropertyName("macAddress")]
	string MacAddress,
	[property: JsonPropertyName("hardwareRevision")]
	string HardwareRevision,
	[property: JsonPropertyName("hardwareBoardType")]
	ushort HardwareBoardType,
	[property: JsonPropertyName("firmwareVersion")]
	string FirmwareVersion,
	[property: JsonPropertyName("firmwareBuildNumber")]
	ushort FirmwareBuildNumber,
	[property: JsonPropertyName("features")]
	IReadOnlyList<string> Features
);
