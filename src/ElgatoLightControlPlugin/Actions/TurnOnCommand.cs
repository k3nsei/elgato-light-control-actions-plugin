namespace Loupedeck.ElgatoLightControlPlugin
{
    using System.Net;

    using ElgatoLightApiClient;

    public class TurnOnCommand : ActionEditorCommand
    {
        public TurnOnCommand()
        {
            this.Name = "TurnOn";
            this.DisplayName = "Enable";
            this.Description = "Turn on the light";
            this.GroupName = "Power State";

            this.ActionEditor.AddControlEx(new ActionEditorTextbox(name: "LightIpAddress", "IP Address:"));
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            var lightIpAddress = actionParameters.GetString("LightIpAddress");

            if (!String.IsNullOrEmpty(lightIpAddress) && IPAddress.TryParse(lightIpAddress, out _))
            {
                _ = Task.Run(() => ElgatoLightApiClient.TurnOn(lightIpAddress));

                return true;
            }

            return base.RunCommand(actionParameters);
        }
    }
}