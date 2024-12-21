namespace ElgatoLightControl.ApiClient.ValueObjects;

using DTO;

using TLightInfo = (
	string ProductName,
	string DisplayName,
	string SerialNumber,
	string MacAddress,
	string HardwareRevision,
	ushort HardwareBoardType,
	string FirmwareVersion,
	ushort FirmwareBuildNumber,
	IReadOnlyList<string> Features
	);

public readonly struct LightInfo(
	string? productName,
	string? displayName,
	string? serialNumber,
	string? macAddress,
	string? hardwareRevision,
	ushort? hardwareBoardType,
	string? firmwareVersion,
	ushort? firmwareBuildNumber,
	IReadOnlyList<string>? features
)
{
	private string ProductName { get; } = productName ?? "Unknown";

	private string DisplayName { get; } = displayName ?? "Elgato Unknown Light";

	private string SerialNumber { get; } = serialNumber ?? "";

	private string MacAddress { get; } = macAddress ?? "00:00:00:00:00:00";

	private string HardwareRevision { get; } = hardwareRevision ?? "0.0";

	private ushort HardwareBoardType { get; } = hardwareBoardType ?? 0;

	private string FirmwareVersion { get; } = firmwareVersion ?? "0.0.0";

	private ushort FirmwareBuildNumber { get; } = firmwareBuildNumber ?? 0;

	private IReadOnlyList<string> Features { get; } = features ?? Array.Empty<string>();

	public static LightInfo Empty => new();

	internal static LightInfo FromDto(LightInfoResponseDto dto) =>
		new(
			dto.ProductName,
			dto.DisplayName,
			dto.SerialNumber,
			dto.MacAddress,
			dto.HardwareRevision,
			dto.HardwareBoardType,
			dto.FirmwareVersion,
			dto.FirmwareBuildNumber,
			dto.Features
		);

	public TLightInfo Value =>
	(
		this.ProductName,
		this.DisplayName,
		this.SerialNumber,
		this.MacAddress,
		this.HardwareRevision,
		this.HardwareBoardType,
		this.FirmwareVersion,
		this.FirmwareBuildNumber,
		this.Features
	);

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
