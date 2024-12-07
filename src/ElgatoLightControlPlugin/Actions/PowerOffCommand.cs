namespace Loupedeck.ElgatoLightControlPlugin
{
    using System.Net;

    using Constants;

    using ElgatoLightControl.ApiClient;

    public class PowerOffCommand : ActionEditorCommand
    {
        public PowerOffCommand()
        {
            this.Name = "PowerOff";
            this.DisplayName = "Power Off";
            this.Description = "This action sends a power-off signal to the Elgato light, effectively turning it off.";
            this.GroupName = ActionGroupName.PowerManagement;

            this.ActionEditor.AddControlEx(new ActionEditorTextbox(name: "LightIpAddress", "IP Address:"));
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            var lightIpAddress = actionParameters.GetString("LightIpAddress");

            if (String.IsNullOrEmpty(lightIpAddress) || !IPAddress.TryParse(lightIpAddress, out _))
            {
                return base.RunCommand(actionParameters);
            }

            _ = Task.Run(() => ApiClient.TurnOff(lightIpAddress));

            return true;

        }
    }
}