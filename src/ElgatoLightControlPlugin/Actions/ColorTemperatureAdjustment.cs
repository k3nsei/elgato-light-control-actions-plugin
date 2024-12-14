namespace Loupedeck.ElgatoLightControlPlugin.Actions;

using System.Net;

using Constants;

using ElgatoLightControl.ApiClient;

public class ColorTemperatureAdjustment : PluginDynamicAdjustment
{
	private static readonly ushort MinValue = 143;

	private static readonly ushort MaxValue = 344;

	private static readonly ushort DefaultValue = MinValue;

	private ushort _currentValue = DefaultValue;

	public ColorTemperatureAdjustment() : base(
		"Adjust color temperature",
		"Adjust color temperature of the light",
		ActionGroupName.Adjustments,
		true
	)
	{
		this.Name = "AdjustColorTemperature";

		this.MakeProfileAction("text;IP Address:");
	}

	protected override void RunCommand(string actionParameter) =>
		this.SetAdjustmentValue(actionParameter, DefaultValue);

	protected override void ApplyAdjustment(string actionParameter, int diff) =>
		this.SetAdjustmentValue(actionParameter, (ushort)(this._currentValue + diff));

	protected override string GetAdjustmentValue(string actionParameter) => this._currentValue.ToString();

	private void SetAdjustmentValue(string actionParameter, ushort value)
	{
		this._currentValue = Math.Clamp(value, MinValue, MaxValue);

		this.AdjustmentValueChanged();

		SetColorTemperature(actionParameter, this._currentValue);
	}

	private static void SetColorTemperature(string ipAddress, ushort colorTemperature)
	{
		if (!IPAddress.TryParse(ipAddress, out _))
		{
			return;
		}

		ApiClient.SetColorTemperature(ipAddress, colorTemperature);
	}

	private static BitmapColor KelvinToBitmapColor(ushort kelvin)
	{
		kelvin = (ushort)(Math.Clamp(kelvin, (ushort)1000, (ushort)40000) / 100);

		var red = kelvin <= 66 ? 255 : Math.Clamp(329.698727446 * Math.Pow(kelvin - 60, -0.1332047592), 0, 255);

		var green = kelvin <= 66
			? Math.Clamp(99.4708025861 * Math.Log(kelvin) - 161.1195681661, 0, 255)
			: Math.Clamp(288.1221695283 * Math.Pow(kelvin - 60, -0.0755148492), 0, 255);

		var blue = kelvin >= 66
			? 255
			: kelvin <= 19
				? 0
				: Math.Clamp(138.5177312231 * Math.Log(kelvin - 10) - 305.0447927307, 0, 255);

		return new BitmapColor((byte)red, (byte)green, (byte)blue);
	}
}
