namespace ElgatoLightApiClient.ValueObjects
{
    using DTO;

    public readonly struct LightInfo
    {
        public required String ProductName { get; init; }
        public required Int16 HardwareBoardType { get; init; }
        public required String HardwareRevision { get; init; }
        public required String MacAddress { get; init; }
        public required Int16 FirmwareBuildNumber { get; init; }
        public required String FirmwareVersion { get; init; }
        public required String SerialNumber { get; init; }
        public required String DisplayName { get; init; }
        public required IReadOnlyList<String> Features { get; init; }

        internal static LightInfo FromDto(LightInfoResponseDto dto)
        {
            return new LightInfo
            {
                ProductName = dto.ProductName,
                HardwareBoardType = dto.HardwareBoardType,
                HardwareRevision = dto.HardwareRevision,
                MacAddress = dto.MacAddress,
                FirmwareBuildNumber = dto.FirmwareBuildNumber,
                FirmwareVersion = dto.FirmwareVersion,
                SerialNumber = dto.SerialNumber,
                DisplayName = dto.DisplayName,
                Features = dto.Features,
            };
        }

        public override String ToString()
        {
            return "LightInfo(" +
                   $"ProductName: {this.ProductName}, " +
                   $"HardwareBoardType: {this.HardwareBoardType}, " +
                   $"HardwareRevision: {this.HardwareRevision}, " +
                   $"MacAddress: {this.MacAddress}, " +
                   $"FirmwareBuildNumber: {this.FirmwareBuildNumber}, " +
                   $"FirmwareVersion: {this.FirmwareVersion}, " +
                   $"SerialNumber: {this.SerialNumber}, " +
                   $"DisplayName: {this.DisplayName}, " +
                   $"Features: [ {String.Join(", ", this.Features)} ]" +
                   ")";
        }
    }
}