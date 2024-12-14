namespace Loupedeck.ElgatoLightControlPlugin.Actions;

using Constants;

using ElgatoLightControl.ApiClient;

using Helpers;

public class PowerToggleFolder : PluginDynamicFolder
{
	private readonly Dictionary<string, (string Name, bool PowerState)> _state = new();

	public PowerToggleFolder()
	{
		this.DisplayName = "Power Toggle";
		this.GroupName = ActionGroupName.PowerManagement;
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
			var powerState = this._state.TryGetValue(ipAddress, out var state) && state.PowerState;

			this._state[ipAddress] = (light.DeviceId, powerState);

			return this.CreateCommandName(ipAddress);
		});

		return new[] { NavigateUpActionName }.Union(actions);
	}

	public override void RunCommand(string actionParameter)
	{
		if (string.IsNullOrWhiteSpace(actionParameter))
		{
			this.Close();
			return;
		}

		var currentPowerState = this._state.TryGetValue(actionParameter, out var state) && state.PowerState;
		var nextPowerState = !currentPowerState;

		this._state[actionParameter] = state with { PowerState = nextPowerState };

		this.CommandImageChanged(actionParameter);

		ToggleLightPowerState(actionParameter, nextPowerState);
	}

	public override string GetCommandDisplayName(string actionParameter, PluginImageSize imageSize)
	{
		if (actionParameter == NavigateUpActionName)
		{
			return base.GetCommandDisplayName(actionParameter, imageSize);
		}

		return this._state.TryGetValue(actionParameter, out var state)
			? state.Name
			: string.Empty;
	}

	public override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
	{
		if (actionParameter == NavigateUpActionName)
		{
			return base.GetCommandImage(actionParameter, imageSize);
		}

		using var bitmapBuilder = new BitmapBuilder(imageSize);

		var (name, image) = this._state.TryGetValue(actionParameter, out var state)
			? (state.Name, state.PowerState
				? EmbeddedResources.ReadImage(ImageId.LightbulbOn)
				: EmbeddedResources.ReadImage(ImageId.LightbulbOff)
			)
			: ("", null);

		if (image is not null)
		{
			bitmapBuilder.DrawImage(
				image,
				(int)(bitmapBuilder.Width * .15),
				0,
				(int)(bitmapBuilder.Width * .7),
				(int)(bitmapBuilder.Height * .7)
			);
		}

		if (!string.IsNullOrWhiteSpace(name))
		{
			bitmapBuilder.DrawText(
				name,
				0,
				(int)(bitmapBuilder.Height * .25),
				bitmapBuilder.Width,
				bitmapBuilder.Height
			);
		}

		return bitmapBuilder.ToImage();
	}

	private static void ToggleLightPowerState(string ipAddress, bool enable) =>
		ApiClient.SetPowerState(ipAddress, enable);
}
