namespace Loupedeck.ElgatoLightControlPlugin
{
    using System.Net;

    using Constants;

    using ElgatoLightControl.ApiClient;

    public class PowerToggleCommand : MultistateActionEditorCommand
    {
        public PowerToggleCommand()
        {
            this.Name = "PowerToggle";
            this.DisplayName = "Toggle power state";
            this.Description = "Toggle the power state of the light";
            this.GroupName = ActionGroupName.PowerManagement;

            this.AddState(PowerState.Off, "Light is turned off");
            this.AddState(PowerState.On, "Light is turned on");

            this.ActionEditor.AddControlEx(new ActionEditorTextbox(name: "LightIpAddress", "IP Address:"));
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            var lightIpAddress = actionParameters.GetString("LightIpAddress");

            if (String.IsNullOrEmpty(lightIpAddress) || !IPAddress.TryParse(lightIpAddress, out _))
            {
                return base.RunCommand(actionParameters);
            }

            this.ToggleCurrentState(actionParameters);

            _ = Task.Run(() =>
                ApiClient.SetPowerState(lightIpAddress,
                    this.GetCurrentState(actionParameters).DisplayName == PowerState.On)
            );

            return true;
        }
    }
}