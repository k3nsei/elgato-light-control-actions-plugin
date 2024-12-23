namespace Loupedeck.ElgatoLightControlPlugin.Actions;

using System.Net;

using Constants;

using ElgatoLightControl.ApiClient;

using Helpers;
using Helpers.Images;

public class PowerToggleFolder : PluginDynamicFolder
{
	private const string AllLights = "__ALL__";

	private readonly Dictionary<string, (string Name, bool PowerState)> _state = new();

	public PowerToggleFolder()
	{
		this.DisplayName = "Power Toggle";
		this.Description = "Toggle the power state of your lights";
		this.GroupName = string.Join(ActionGroupName.Separator, "Folders", ActionGroupName.PowerManagement);

		var subscription = PluginDeviceManager.DevicesObservable.Subscribe(devices =>
		{
			foreach (var entry in devices)
			{
				var key = entry.IpAddress.ToString();

				this._state[key] = (entry.LightInfo.Value.DisplayName, entry.LightState.Value.PowerState);

				this.AdjustmentImageChanged(key);
				this.CommandImageChanged(key);
			}

			this.AdjustmentImageChanged(AllLights);
			this.CommandImageChanged(AllLights);

			this.ButtonActionNamesChanged();
			this.EncoderActionNamesChanged();
		});

		(this.Plugin as ElgatoLightControlPlugin)?.CancellationToken.Register(subscription.Dispose);
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

	public override IEnumerable<string> GetEncoderPressActionNames(DeviceType deviceType) =>
		DevicesWithEncoders.Supported.HasFlag(deviceType)
			? this.GetButtonPressActionNames(deviceType).Skip(1)
			: [];

	public override IEnumerable<string> GetEncoderRotateActionNames(DeviceType deviceType)
	{
		var actions = this._state.Keys.Select(this.CreateAdjustmentName).ToList();

		if (actions.Count > 0)
		{
			actions.Insert(0, this.CreateAdjustmentName(AllLights));
		}

		return actions;
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

	public override void ApplyAdjustment(string actionParameter, int diff)
	{
		if (!IPAddress.TryParse(actionParameter, out _) && actionParameter != AllLights)
		{
			base.ApplyAdjustment(actionParameter, diff);
			return;
		}

		var nextPowerState = diff > 0;

		if (actionParameter == AllLights)
		{
			this.ToggleAllLights(nextPowerState);
			return;
		}

		this.ToggleLight(actionParameter, nextPowerState);
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

	public override string GetAdjustmentDisplayName(string actionParameter, PluginImageSize imageSize) =>
		this.GetCommandDisplayName(actionParameter, imageSize);

	public override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
	{
		if (actionParameter == NavigateUpActionName)
		{
			return base.GetCommandImage(actionParameter, imageSize);
		}

		var (name, type) =
			actionParameter == AllLights
				? this._state.Values.Any(x => x.PowerState)
					? ("All lights", PowerToggleImage.AllLightsOn)
					: ("All lights", PowerToggleImage.AllLightsOff)
				: this._state.TryGetValue(actionParameter, out var state)
					? (state.Name, state.PowerState
						? PowerToggleImage.LightOn
						: PowerToggleImage.LightOff
					)
					: ("", null);

		return PowerToggleImage.ToImage(name, type, imageSize);
	}

	public override BitmapImage GetAdjustmentImage(string actionParameter, PluginImageSize imageSize) =>
		this.GetCommandImage(actionParameter, imageSize);

	private void ToggleAllLights(bool? forcedPowerState = null)
	{
		var currentPowerState = this._state.Values.Any(x => x.PowerState);
		var nextPowerState = forcedPowerState ?? !currentPowerState;

		this._state.Keys.ToList().ForEach((ipAddress) =>
		{
			this._state[ipAddress] = this._state[ipAddress] with { PowerState = nextPowerState };

			this.CommandImageChanged(ipAddress);

			ApiClient.SetPowerState(ipAddress, nextPowerState);
		});

		this.CommandImageChanged(AllLights);
	}

	private void ToggleLight(string ipAddress, bool? forcedPowerState = null)
	{
		var currentPowerState = this._state.TryGetValue(ipAddress, out var state) && state.PowerState;
		var nextPowerState = forcedPowerState ?? !currentPowerState;

		this._state[ipAddress] = state with { PowerState = nextPowerState };

		this.CommandImageChanged(AllLights);
		this.CommandImageChanged(ipAddress);

		ApiClient.SetPowerState(ipAddress, nextPowerState);
	}
}
