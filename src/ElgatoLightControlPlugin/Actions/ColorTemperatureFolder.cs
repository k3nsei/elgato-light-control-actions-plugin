namespace Loupedeck.ElgatoLightControlPlugin.Actions;

using Constants;

using ElgatoLightControl.ApiClient;

using Helpers;

public class ColorTemperatureFolder : PluginDynamicFolder
{
	private readonly Dictionary<string, (string Name, ushort ColorTemperature)> _state = new();

	public ColorTemperatureFolder()
	{
		this.DisplayName = "Color Temperature";
		this.Description = "Adjust the color temperature of your lights";
		this.GroupName = ActionGroupName.Adjustments;
	}

	public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType deviceType) =>
		PluginDynamicFolderNavigation.ButtonArea;

	public override BitmapImage GetButtonImage(PluginImageSize imageSize) =>
		FolderImage.ToImage(ImageId.TemperatureFolder, imageSize);

	public override IEnumerable<string> GetButtonPressActionNames(DeviceType deviceType)
	{
		var lights = PluginDeviceManager.Devices;

		var actions = lights.Select(light =>
		{
			var ipAddress = light.IPAddress.ToString();
			var colorTemperature = this._state.TryGetValue(ipAddress, out var state) ? state.ColorTemperature : (ushort)143;

			this._state[ipAddress] = (light.DeviceId, colorTemperature);

			return this.CreateAdjustmentName(ipAddress);
		});

		return new[] { NavigateUpActionName }.Union(actions);
	}

	public override void ApplyAdjustment(string actionParameter, int diff)
	{
		if (string.IsNullOrWhiteSpace(actionParameter))
		{
			return;
		}

		var curr = this._state.TryGetValue(actionParameter, out var state) ? state.ColorTemperature : (ushort)143;
		var next = (ushort)Math.Clamp(curr + diff, 143, 344);

		this._state[actionParameter] = state with { ColorTemperature = next };

		this.AdjustmentImageChanged(actionParameter);

		SetColorTemperature(actionParameter, next);
	}

	public override string GetAdjustmentDisplayName(string actionParameter, PluginImageSize imageSize)
	{
		if (actionParameter == NavigateUpActionName)
		{
			return base.GetAdjustmentDisplayName(actionParameter, imageSize);
		}

		return this._state.TryGetValue(actionParameter, out var state)
			? $"{MiredToKelvin(state.ColorTemperature)}K"
			: "Unknown";
	}

	public override BitmapImage GetAdjustmentImage(string actionParameter, PluginImageSize imageSize)
	{
		if (actionParameter == NavigateUpActionName)
		{
			return base.GetAdjustmentImage(actionParameter, imageSize);
		}

		var mired = this._state.TryGetValue(actionParameter, out var state) ? state.ColorTemperature : (ushort)143;
		var kelvin = MiredToKelvin(mired);

		using var bitmapBuilder = new BitmapBuilder(imageSize);

		var size = Math.Min(bitmapBuilder.Width, bitmapBuilder.Height);

		bitmapBuilder.FillRectangle(
			0,
			0,
			bitmapBuilder.Width,
			bitmapBuilder.Height,
			BitmapColor.Black
		);

		bitmapBuilder.FillCircle(
			size * .5f,
			size * .5f,
			size * .5f,
			KelvinToColor(kelvin)
		);

		bitmapBuilder.DrawText(
			$"{kelvin}K",
			BitmapColor.Black
		);

		return bitmapBuilder.ToImage();
	}

	private static void SetColorTemperature(string ipAddress, ushort colorTemperature) =>
		ApiClient.SetColorTemperature(ipAddress, colorTemperature);

	private static ushort MiredToKelvin(ushort value) => (ushort)(Math.Round(1000000d / value / 50d) * 50);

	private static BitmapColor KelvinToColor(ushort kelvin)
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
