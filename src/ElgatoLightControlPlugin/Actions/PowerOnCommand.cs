namespace Loupedeck.ElgatoLightControlPlugin
{
    using System.Net;

    using Constants;

    using ElgatoLightApiClient;

    public class PowerOnCommand : ActionEditorCommand
    {
        public PowerOnCommand()
        {
            this.Name = "TurnOn";
            this.DisplayName = "Power On";
            this.Description = "This action sends a power-on signal to the Elgato light, effectively turning it on.";
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

            _ = Task.Run(() => ElgatoLightApiClient.TurnOn(lightIpAddress));

            return true;
        }
    }
}