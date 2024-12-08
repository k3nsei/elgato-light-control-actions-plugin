namespace Loupedeck.ElgatoLightControlPlugin.Actions
{
    using Constants;

    using ElgatoLightControl.ApiClient;

    using Helpers;

    public class PowerToggleActionFolder : PluginDynamicFolder
    {
        private readonly Dictionary<String, (String Name, Boolean PowerState)> _state = new();

        public PowerToggleActionFolder()
        {
            this.DisplayName = "Power Toggle";
            this.GroupName = ActionGroupName.PowerManagement;
        }

        public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType deviceType) =>
            PluginDynamicFolderNavigation.ButtonArea;

        public override BitmapImage GetButtonImage(PluginImageSize imageSize) =>
            EmbeddedResources.ReadImage(ImageId.Devices);

        public override IEnumerable<String> GetButtonPressActionNames(DeviceType _deviceType)
        {
            var devices = PluginDeviceManager.Devices;

            var actions = devices.Select((device) =>
            {
                var ipAddress = device.IPAddress.ToString();
                var powerState = this._state.TryGetValue(ipAddress, out var state) && state.PowerState;

                this._state[ipAddress] = (device.DeviceId, powerState);

                return this.CreateCommandName(ipAddress);
            });

            return new[] { NavigateUpActionName }.Union(actions);
        }

        public override void RunCommand(String actionParameter)
        {
            if (String.IsNullOrWhiteSpace(actionParameter))
            {
                return;
            }

            var currentPowerState = this._state.TryGetValue(actionParameter, out var state) && state.PowerState;
            var nextPowerState = !currentPowerState;

            this._state[actionParameter] = state with { PowerState = nextPowerState };

            this.CommandImageChanged(actionParameter);

            this.Close();

            ToggleLightPowerState(actionParameter, nextPowerState);
        }

        public override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter == NavigateUpActionName)
            {
                return base.GetCommandDisplayName(actionParameter, imageSize);
            }

            return this._state.TryGetValue(actionParameter, out var state)
                ? state.Name
                : String.Empty;
        }

        public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter == NavigateUpActionName)
            {
                return base.GetCommandImage(actionParameter, imageSize);
            }

            return this._state.TryGetValue(actionParameter, out var state)
                ? state.PowerState
                    ? EmbeddedResources.ReadImage(ImageId.LightbulbOn)
                    : EmbeddedResources.ReadImage(ImageId.LightbulbOff)
                : null;
        }

        private static void ToggleLightPowerState(String ipAddress, Boolean enable)
        {
            new Thread(() => ApiClient.SetPowerState(ipAddress, enable)).Start();
        }
    }
}