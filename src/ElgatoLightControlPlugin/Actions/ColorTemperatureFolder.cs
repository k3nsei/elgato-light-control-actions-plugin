namespace Loupedeck.ElgatoLightControlPlugin.Actions;

using Constants;

using ElgatoLightControl.ApiClient;
using ElgatoLightControl.ApiClient.Helpers;

using Helpers;

public class ColorTemperatureFolder : PluginDynamicFolder
{
	private readonly Dictionary<string, (string Name, ushort ColorTemperature)> _state = new();

	public ColorTemperatureFolder()
	{
		this.DisplayName = "Color Temperature";
		this.Description = "Adjust the color temperature of your lights";
		this.GroupName = ActionGroupName.Adjustments;

		PluginDeviceManager.DevicesObservable.Subscribe(devices =>
		{
			foreach (var entry in devices)
			{
				var key = entry.IpAddress.ToString();

				this._state[key] = (entry.LightInfo.Value.DisplayName, entry.LightState.Value.ColorTemperature);

				this.AdjustmentValueChanged(key);
				this.AdjustmentImageChanged(key);
			}
		});
	}

	public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType deviceType) =>
		PluginDynamicFolderNavigation.ButtonArea;

	public override BitmapImage GetButtonImage(PluginImageSize imageSize) =>
		FolderImage.ToImage(ImageId.TemperatureFolder, imageSize);

	public override IEnumerable<string> GetButtonPressActionNames(DeviceType deviceType)
	{
		var actions = this._state.Keys.Select(this.CreateAdjustmentName);

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

		ApiClient.SetColorTemperature(actionParameter, next);
	}

	public override string GetAdjustmentDisplayName(string actionParameter, PluginImageSize imageSize)
	{
		if (actionParameter == NavigateUpActionName)
		{
			return base.GetAdjustmentDisplayName(actionParameter, imageSize);
		}

		return this._state.TryGetValue(actionParameter, out var state)
			? $"{TemperatureConverter.MiredToKelvin(state.ColorTemperature)}K"
			: "Unknown";
	}

	public override BitmapImage GetAdjustmentImage(string actionParameter, PluginImageSize imageSize)
	{
		if (actionParameter == NavigateUpActionName)
		{
			return base.GetAdjustmentImage(actionParameter, imageSize);
		}

		var mired = this._state.TryGetValue(actionParameter, out var state) ? state.ColorTemperature : (ushort)143;

		return ColorTemperatureImage.ToImage(mired, imageSize);
	}
}
