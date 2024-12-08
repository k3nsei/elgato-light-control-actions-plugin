namespace Loupedeck.ElgatoLightControlPlugin.Actions
{
    using Constants;

    using ElgatoLightControl.ApiClient;

    public class PowerOnOffCommand : PluginTwoStateDynamicCommand
    {
        public static readonly String CommandName = "PowerOnOff";

        private readonly Lazy<BitmapImage> _imageOff = new Lazy<BitmapImage>(() =>
            EmbeddedResources.ReadImage(EmbeddedResources.FindFile(ImageId.LightbulbOff)));

        private readonly Lazy<BitmapImage> _imageOn = new Lazy<BitmapImage>(() =>
            EmbeddedResources.ReadImage(EmbeddedResources.FindFile(ImageId.LightbulbOn)));

        public PowerOnOffCommand() : base(
            displayName: "Power Light On/Off",
            description: "Toggle the light's power state",
            groupName: ActionGroupName.Switches
        )
        {
            this.Name = CommandName;

            this.AddTurnOffCommand(
                PowerState.Off,
                this._imageOff.Value
            );

            this.AddTurnOnCommand(
                PowerState.On,
                this._imageOn.Value
            );

            this.AddToggleCommand(
                PowerState.Toggle,
                this._imageOn.Value,
                this._imageOff.Value
            );
        }

        protected override void RunCommand(String actionParameter)
        {
            if (Byte.TryParse(actionParameter, out _))
            {
                base.RunCommand(actionParameter);
                return;
            }

            base.RunCommand(TwoStateCommand.Toggle);

            try
            {
                var enable = this.GetCurrentState().DisplayName == PowerState.On;

                this.ToggleLightPowerState(actionParameter, enable);
            }
            catch
            {
                this.ToggleLightPowerState(actionParameter, false);
            }
        }

        private void ToggleLightPowerState(String ipAddress, Boolean enable)
        {
            _ = Task.Run(() => ApiClient.SetPowerState(ipAddress, enable));
        }
    }
}