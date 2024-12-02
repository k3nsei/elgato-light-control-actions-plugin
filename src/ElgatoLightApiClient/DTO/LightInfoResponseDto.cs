namespace ElgatoLightApiClient.DTO
{
    using System.Text.Json.Serialization;

    internal record LightInfoResponseDto(
        [property: JsonPropertyName("productName")]
        String ProductName,
        [property: JsonPropertyName("hardwareBoardType")]
        Int16 HardwareBoardType,
        [property: JsonPropertyName("hardwareRevision")]
        String HardwareRevision,
        [property: JsonPropertyName("macAddress")]
        String MacAddress,
        [property: JsonPropertyName("firmwareBuildNumber")]
        Int16 FirmwareBuildNumber,
        [property: JsonPropertyName("firmwareVersion")]
        String FirmwareVersion,
        [property: JsonPropertyName("serialNumber")]
        String SerialNumber,
        [property: JsonPropertyName("displayName")]
        String DisplayName,
        [property: JsonPropertyName("features")]
        IReadOnlyList<String> Features
    );
}