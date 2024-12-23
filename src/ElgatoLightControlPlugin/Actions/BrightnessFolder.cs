namespace Loupedeck.ElgatoLightControlPlugin.Actions;

using Constants;

using ElgatoLightControl.ApiClient;

using Helpers;
using Helpers.Images;

public class BrightnessFolder : PluginDynamicFolder
{
	private readonly Dictionary<string, (string Name, byte Brightness)> _state = new();

	public BrightnessFolder()
	{
		this.DisplayName = "Brightness";
		this.Description = "Adjust the brightness of your lights";
		this.GroupName = string.Join(ActionGroupName.Separator, "Folders", ActionGroupName.Adjustments);

		var subscription = PluginDeviceManager.DevicesObservable.Subscribe(devices =>
		{
			foreach (var entry in devices)
			{
				var key = entry.IpAddress.ToString();

				this._state[key] = (entry.LightInfo.Value.DisplayName, entry.LightState.Value.Brightness);

				this.AdjustmentValueChanged(key);
				this.AdjustmentImageChanged(key);
			}

			this.ButtonActionNamesChanged();
			this.EncoderActionNamesChanged();
		});

		(this.Plugin as ElgatoLightControlPlugin)?.CancellationToken.Register(subscription.Dispose);
	}

	public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType deviceType) =>
		PluginDynamicFolderNavigation.ButtonArea;

	public override BitmapImage GetButtonImage(PluginImageSize imageSize) =>
		FolderImage.ToImage(ImageId.BrightnessFolder, imageSize);

	public override IEnumerable<string> GetButtonPressActionNames(DeviceType deviceType)
	{
		var actions = this._state.Keys.Select(this.CreateAdjustmentName);

		return new[] { NavigateUpActionName }.Union(actions);
	}

	public override IEnumerable<string> GetEncoderPressActionNames(DeviceType deviceType) =>
		DevicesWithEncoders.Supported.HasFlag(deviceType)
			? this.GetButtonPressActionNames(deviceType).Skip(1)
			: [];

	public override IEnumerable<string> GetEncoderRotateActionNames(DeviceType deviceType) =>
		this.GetEncoderPressActionNames(deviceType);

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

		ApiClient.SetBrightness(actionParameter, next);
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

		return BrightnessImage.ToImage(brightness, imageSize);
	}
}
