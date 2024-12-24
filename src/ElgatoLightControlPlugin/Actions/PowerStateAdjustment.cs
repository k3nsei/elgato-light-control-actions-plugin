namespace Loupedeck.ElgatoLightControlPlugin.Actions;

using Constants;

using ElgatoLightControl.ApiClient;

using Helpers;
using Helpers.Images;

public class PowerStateAdjustment : PluginDynamicAdjustment
{
	private readonly Dictionary<string, (string Name, bool PowerState)> _state = new();

	public PowerStateAdjustment() : base(hasReset: true)
	{
		this.Name = "PowerStateAdjustment";
		this.DisplayName = "Power State";
		this.Description = "Turns the light on or off";
		this.GroupName = ActionGroupName.PowerManagement;

		var subscription = PluginDeviceManager.DevicesObservable.Subscribe(devices =>
		{
			foreach (var entry in devices)
			{
				var key = entry.IpAddress.ToString();

				this._state[key] = (entry.LightInfo.Value.DisplayName, entry.LightState.Value.PowerState);

				this.AddParameter(key, entry.LightInfo.Value.DisplayName, ActionGroupName.PowerManagement);
			}
		});

		(this.Plugin as ElgatoLightControlPlugin)?.CancellationToken.Register(subscription.Dispose);
	}

	protected override BitmapImage GetAdjustmentImage(string actionParameter, PluginImageSize imageSize) {
		var state = this._state[actionParameter];

		return PowerStateImage.ToImage(
			state.Name,
			state.PowerState
				? PowerStateImage.LightOn
				: PowerStateImage.LightOff,
			imageSize
		);
	}

	protected override void RunCommand(string actionParameter)
	{
		var state = this._state[actionParameter];
		var nextPowerState = !state.PowerState;

		this.ChangePowerState(actionParameter, nextPowerState);
	}

	protected override void ApplyAdjustment(string actionParameter, int diff)
	{
		var nextPowerState = diff > 0;

		this.ChangePowerState(actionParameter, nextPowerState);
	}

	private void ChangePowerState(string ipAddress, bool powerState)
	{
		this._state[ipAddress] = this._state[ipAddress] with { PowerState = powerState };

		this.ActionImageChanged();

		ApiClient.SetPowerState(ipAddress, powerState);
	}
}
