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
		this.Description = "Toggle the power state of your lights";
		this.GroupName = ActionGroupName.PowerManagement;

		PluginDeviceManager.DevicesObservable.Subscribe(devices =>
		{
			foreach (var entry in devices)
			{
				var key = entry.IpAddress.ToString();

				this._state[key] = (entry.LightInfo.Value.DisplayName, entry.LightState.Value.PowerState);

				this.AdjustmentValueChanged(key);
				this.AdjustmentImageChanged(key);
			}
		});
	}

	public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType deviceType) =>
		PluginDynamicFolderNavigation.ButtonArea;

	public override BitmapImage GetButtonImage(PluginImageSize imageSize) =>
		FolderImage.ToImage(ImageId.LightbulbFolder, imageSize);

	public override IEnumerable<string> GetButtonPressActionNames(DeviceType deviceType)
	{
		var actions = this._state.Keys.Select(this.CreateCommandName).ToList();

		if (actions.Count > 0)
		{
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

		if (actionParameter == AllLights)
		{
			this.ToggleAllLights();
			return;
		}

		this.ToggleLight(actionParameter);
	}

	public override string GetCommandDisplayName(string actionParameter, PluginImageSize imageSize)
	{
		if (actionParameter == NavigateUpActionName)
		{
			return base.GetCommandDisplayName(actionParameter, imageSize);
		}

		if (actionParameter == AllLights)
		{
			return this._state.Values.Any(x => x.PowerState)
				? "Turn off all lights"
				: "Turn on all lights";
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

		var (name, image) =
			actionParameter == AllLights
				? this._state.Values.Any(x => x.PowerState)
					? ("All lights", EmbeddedResources.ReadImage(ImageId.LightbulbGroupOn))
					: ("All lights", EmbeddedResources.ReadImage(ImageId.LightbulbGroupOff))
				: this._state.TryGetValue(actionParameter, out var state)
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

	private void ToggleAllLights()
	{
		var currentPowerState = this._state.Values.Any(x => x.PowerState);
		var nextPowerState = !currentPowerState;

		this._state.Keys.ToList().ForEach((ipAddress) =>
		{
			this._state[ipAddress] = this._state[ipAddress] with { PowerState = nextPowerState };

			this.CommandImageChanged(ipAddress);

			ApiClient.SetPowerState(ipAddress, nextPowerState);
		});

		this.CommandImageChanged(AllLights);
	}

	private void ToggleLight(string ipAddress)
	{
		var currentPowerState = this._state.TryGetValue(ipAddress, out var state) && state.PowerState;
		var nextPowerState = !currentPowerState;

		this._state[ipAddress] = state with { PowerState = nextPowerState };

		this.CommandImageChanged(AllLights);
		this.CommandImageChanged(ipAddress);

		ApiClient.SetPowerState(ipAddress, nextPowerState);
	}
}
