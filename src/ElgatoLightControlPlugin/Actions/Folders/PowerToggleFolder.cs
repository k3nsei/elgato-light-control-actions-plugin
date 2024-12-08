namespace Loupedeck.ElgatoLightControlPlugin.Actions
{
    using Constants;

    using Helpers;

    public class PowerToggleFolder : PluginDynamicFolder
    {
        public PowerToggleFolder()
        {
            this.DisplayName = "Power Toggle";
            this.GroupName = ActionGroupName.PowerManagement;

            PluginDeviceManager.DeviceListChanged += OnDeviceListChanged;

            void OnDeviceListChanged(Object _, DeviceListChangedEventArgs e) => this.ButtonActionNamesChanged();
        }

        public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType deviceType) =>
            PluginDynamicFolderNavigation.None;

        public override IEnumerable<String> GetButtonPressActionNames(DeviceType _deviceType)
        {
            var devices = PluginDeviceManager.Devices;

            var actions = devices.Select(
                device => ActionString.ToString(
                    ElgatoLightControlPlugin.PluginName,
                    PowerOnOffCommand.CommandName,
                    device.IPAddress.ToString()
                )
            );

            return new[] { NavigateUpActionName }.Union(actions);
        }
    }
}