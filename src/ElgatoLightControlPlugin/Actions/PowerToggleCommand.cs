namespace Loupedeck.ElgatoLightControlPlugin
{
    using System.Net;

    using ElgatoLightApiClient;

    public class PowerToggleCommand : MultistateActionEditorCommand
    {
        public PowerToggleCommand(): base()
        {
            this.Name = "PowerToggle";
            this.DisplayName = "Toggle power state";
            this.Description = "Toggle the power state of the light";
            this.GroupName = "Power State";

            this.AddState("On", "Light is turned on");
            this.AddState("Off", "Light is turned off");

            this.ActionEditor.AddControlEx(new ActionEditorTextbox(name: "LightIpAddress", "IP Address:"));
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            var lightIpAddress = actionParameters.GetString("LightIpAddress");

            if (!String.IsNullOrEmpty(lightIpAddress) && IPAddress.TryParse(lightIpAddress, out _))
            {
                this.ToggleCurrentState(actionParameters);

                var state = this.GetCurrentState(actionParameters);

                _ = Task.Run(() =>
                    state.DisplayName == "Off"
                        ? ElgatoLightApiClient.TurnOn(lightIpAddress)
                        : ElgatoLightApiClient.TurnOff(lightIpAddress)
                );

                return true;
            }

            return base.RunCommand(actionParameters);
        }
    }
}