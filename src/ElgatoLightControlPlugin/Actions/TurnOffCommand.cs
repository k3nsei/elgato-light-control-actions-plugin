namespace Loupedeck.ElgatoLightControlPlugin
{
    using System.Net;

    using ElgatoLightApiClient;

    public class TurnOffCommand : ActionEditorCommand
    {
        public TurnOffCommand()
        {
            this.Name = "TurnOff";
            this.DisplayName = "Disable";
            this.Description = "Turn off the light";
            this.GroupName = "Power State";

            this.ActionEditor.AddControlEx(new ActionEditorTextbox(name: "LightIpAddress", "IP Address:"));
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            var lightIpAddress = actionParameters.GetString("LightIpAddress");

            if (!String.IsNullOrEmpty(lightIpAddress) && IPAddress.TryParse(lightIpAddress, out _))
            {
                _ = Task.Run(() => ElgatoLightApiClient.TurnOff(lightIpAddress));

                return true;
            }

            return base.RunCommand(actionParameters);
        }
    }
}