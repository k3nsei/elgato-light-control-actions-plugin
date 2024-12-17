namespace Loupedeck.ElgatoLightControlPlugin.Actions;

using Constants;

using ElgatoLightControl.ApiClient;

using Helpers;

public class PowerToggleFolder : PluginDynamicFolder
{
	private static readonly string AllLights = "__ALL__";

	private readonly Dictionary<string, (string Name, bool PowerState)> _state = new();

	public PowerToggleFolder()
	{
		this.DisplayName = "Power Toggle";
		this.GroupName = ActionGroupName.PowerManagement;
	}

	public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType deviceType) =>
		PluginDynamicFolderNavigation.ButtonArea;

	public override BitmapImage GetButtonImage(PluginImageSize imageSize) =>
		FolderImage.ToImage(ImageId.LightbulbFolder, imageSize);

	public override IEnumerable<string> GetButtonPressActionNames(DeviceType deviceType)
	{
		var lights = PluginDeviceManager.Devices;

		var actions = lights.Select(light =>
		{
			var ipAddress = light.IPAddress.ToString();
			var powerState = this._state.TryGetValue(ipAddress, out var state) && state.PowerState;

			this._state[ipAddress] = (light.DeviceId, powerState);

			return this.CreateCommandName(ipAddress);
		}).ToList();

		if (actions.Count > 0)
		{
			this._state[AllLights] = ("Toggle All", false);

			actions.Insert(0, this.CreateCommandName(AllLights));
		}

		return new[] { NavigateUpActionName }.Union(actions);
	}

	public override void RunCommand(string actionParameter)
	{
		if (string.IsNullOrWhiteSpace(actionParameter))
		{
			this.Close();
			return;
		}

		if (actionParameter == NavigateUpActionName)
		{
			base.RunCommand(actionParameter);
			return;
		}

		var currentPowerState = this._state.TryGetValue(actionParameter, out var state) && state.PowerState;
		var nextPowerState = !currentPowerState;

		this._state[actionParameter] = state with { PowerState = nextPowerState };

		if (actionParameter == AllLights)
		{
			this._state
				.Where(entry => entry.Key != AllLights).ToList()
				.ForEach(entry => this.RunCommand(entry.Key));
			return;
		}

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
				? actionParameter == AllLights
					? EmbeddedResources.ReadImage(ImageId.LightbulbGroupOn)
					: EmbeddedResources.ReadImage(ImageId.LightbulbOn)
				: actionParameter == AllLights
					? EmbeddedResources.ReadImage(ImageId.LightbulbGroupOff)
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
