namespace ElgatoLightControl.ApiClient.ValueObjects;

using DTO;

public readonly struct LightInfo
{
	private string ProductName { get; init; }

	private string DisplayName { get; init; }

	private string SerialNumber { get; init; }

	private string MacAddress { get; init; }

	private string HardwareRevision { get; init; }

	private ushort HardwareBoardType { get; init; }

	private string FirmwareVersion { get; init; }

	private ushort FirmwareBuildNumber { get; init; }

	private IReadOnlyList<string> Features { get; init; }

	public static LightInfo Empty => new()
	{
		ProductName = string.Empty,
		DisplayName = string.Empty,
		SerialNumber = string.Empty,
		MacAddress = string.Empty,
		HardwareRevision = string.Empty,
		HardwareBoardType = 0,
		FirmwareVersion = string.Empty,
		FirmwareBuildNumber = 0,
		Features = Array.Empty<string>()
	};

	public static LightInfo FromDto(LightInfoResponseDto dto) =>
		new()
		{
			ProductName = dto.ProductName,
			DisplayName = dto.DisplayName,
			SerialNumber = dto.SerialNumber,
			MacAddress = dto.MacAddress,
			HardwareRevision = dto.HardwareRevision,
			HardwareBoardType = dto.HardwareBoardType,
			FirmwareVersion = dto.FirmwareVersion,
			FirmwareBuildNumber = dto.FirmwareBuildNumber,
			Features = dto.Features
		};

	public (string ProductName, string DisplayName, string SerialNumber, string MacAddress, string
		HardwareRevision, ushort HardwareBoardType, string FirmwareVersion, ushort FirmwareBuildNumber,
		IReadOnlyList<string> Features) Value =>
		(this.ProductName, this.DisplayName, this.SerialNumber, this.MacAddress, this.HardwareRevision,
			this.HardwareBoardType, this.FirmwareVersion, this.FirmwareBuildNumber, this.Features);

	public override string ToString() =>
		"LightInfo( " +
		$"Product Name: {this.ProductName}, " +
		$"Display Name: {this.DisplayName}, " +
		$"Serial Number: {this.SerialNumber}, " +
		$"Mac Address: {this.MacAddress}, " +
		$"Hardware Revision: {this.HardwareRevision}, " +
		$"Hardware Board Type: {this.HardwareBoardType}, " +
		$"Firmware Version: {this.FirmwareVersion}, " +
		$"Firmware Build Number: {this.FirmwareBuildNumber}, " +
		$"Features: ( {string.Join(", ", this.Features)} ) " +
		")";
}
