namespace Loupedeck.ElgatoLightControlPlugin.Actions
{
    using Constants;

    using ElgatoLightControl.ApiClient;

    using Helpers;

    public class BrightnessAdjustmentFolder : PluginDynamicFolder
    {
        private readonly Dictionary<String, (String Name, Byte Brightness)> _state = new();

        public BrightnessAdjustmentFolder()
        {
            this.DisplayName = "Brightness";
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
                var brightness = this._state.TryGetValue(ipAddress, out var state) ? state.Brightness : (Byte)1;

                this._state[ipAddress] = (device.DeviceId, brightness);

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

            var curr = this._state.TryGetValue(actionParameter, out var state) ? state.Brightness : (Byte)1;
            var next = (Byte)Math.Clamp(curr + diff, 1, 100);

            this._state[actionParameter] = state with { Brightness = next };

            this.CommandImageChanged(actionParameter);

            SetBrightness(actionParameter, next);
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

        private static void SetBrightness(String ipAddress, Byte brightness)
        {
            new Thread(() => ApiClient.SetBrightness(ipAddress, brightness)).Start();
        }
    }
}