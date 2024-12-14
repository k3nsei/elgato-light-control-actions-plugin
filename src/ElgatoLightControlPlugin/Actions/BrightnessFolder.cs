namespace Loupedeck.ElgatoLightControlPlugin.Actions;

using Constants;

using ElgatoLightControl.ApiClient;

using Helpers;

public class BrightnessFolder : PluginDynamicFolder
{
	private readonly Dictionary<string, (string Name, byte Brightness)> _state = new();

	public BrightnessFolder()
	{
		this.DisplayName = "Brightness";
		this.Description = "Adjust the brightness of your lights";
		this.GroupName = ActionGroupName.Adjustments;
	}

	public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType deviceType) =>
		PluginDynamicFolderNavigation.ButtonArea;

	public override BitmapImage GetButtonImage(PluginImageSize imageSize) =>
		EmbeddedResources.ReadImage(ImageId.Devices);

	public override IEnumerable<string> GetButtonPressActionNames(DeviceType deviceType)
	{
		var lights = PluginDeviceManager.Devices;

		var actions = lights.Select(light =>
		{
			var ipAddress = light.IPAddress.ToString();
			var brightness = this._state.TryGetValue(ipAddress, out var state) ? state.Brightness : (byte)0;

			this._state[ipAddress] = (light.DeviceId, brightness);

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

		var curr = this._state.TryGetValue(actionParameter, out var state) ? state.Brightness : (byte)0;
		var next = (byte)Math.Clamp(curr + diff, 0, 100);

		this._state[actionParameter] = state with { Brightness = next };

		this.AdjustmentImageChanged(actionParameter);

		SetBrightness(actionParameter, next);
	}

	public override string GetAdjustmentDisplayName(string actionParameter, PluginImageSize imageSize)
	{
		if (actionParameter == NavigateUpActionName)
		{
			return base.GetAdjustmentDisplayName(actionParameter, imageSize);
		}

		return this._state.TryGetValue(actionParameter, out var state)
			? $"{state.Brightness}%"
			: "Unknown";
	}

	public override BitmapImage GetAdjustmentImage(string actionParameter, PluginImageSize imageSize)
	{
		if (actionParameter == NavigateUpActionName)
		{
			return base.GetAdjustmentImage(actionParameter, imageSize);
		}

		var brightness = this._state.TryGetValue(actionParameter, out var state) ? state.Brightness : (byte)0;

		using var bitmapBuilder = new BitmapBuilder(imageSize);

		var size = Math.Min(bitmapBuilder.Width, bitmapBuilder.Height);
		var alpha = (byte)Math.Round(150 + brightness / 100f * (255 - 150));

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
			new BitmapColor(255, 255, 255, alpha)
		);

		bitmapBuilder.DrawText(
			$"{brightness}%",
			BitmapColor.Black
		);

		return bitmapBuilder.ToImage();
	}

	private static void SetBrightness(string ipAddress, byte brightness) =>
		ApiClient.SetBrightness(ipAddress, brightness);
}
