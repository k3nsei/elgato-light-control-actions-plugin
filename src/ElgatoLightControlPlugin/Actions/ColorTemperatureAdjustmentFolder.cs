namespace Loupedeck.ElgatoLightControlPlugin.Actions
{
    using Constants;

    using ElgatoLightControl.ApiClient;

    using Helpers;

    public class ColorTemperatureAdjustmentFolder : PluginDynamicFolder
    {
        private readonly Dictionary<String, (String Name, UInt16 ColorTemperature)> _state = new();

        public ColorTemperatureAdjustmentFolder()
        {
            this.DisplayName = "Color Temperature";
            this.GroupName = ActionGroupName.Adjustments;
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
                var colorTemperature = this._state.TryGetValue(ipAddress, out var state) ? state.ColorTemperature : (UInt16)143;

                this._state[ipAddress] = (device.DeviceId, colorTemperature);

                return this.CreateAdjustmentName(ipAddress);
            });

            return new[] { NavigateUpActionName }.Union(actions);
        }

        public override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            if (String.IsNullOrWhiteSpace(actionParameter))
            {
                return;
            }

            var curr = this._state.TryGetValue(actionParameter, out var state) ? state.ColorTemperature : (UInt16)143;
            var next = (UInt16)Math.Clamp(curr + diff, 143, 344);

            this._state[actionParameter] = state with { ColorTemperature = next };

            this.CommandImageChanged(actionParameter);

            SetColorTemperature(actionParameter, next);
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

        private static void SetColorTemperature(String ipAddress, UInt16 colorTemperature)
        {
            new Thread(() => ApiClient.SetColorTemperature(ipAddress, colorTemperature)).Start();
        }
    }
}