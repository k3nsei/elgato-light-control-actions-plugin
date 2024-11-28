namespace Loupedeck.ElgatoLightControlPlugin
{
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using ElgatoLightApiClient;

    public class BrightnessAdjustment : ActionEditorAdjustment
    {
        private readonly BehaviorSubject<(String LightIpAddress, Byte Brightness)> _brightnessSubject = new(("", 25));

        public BrightnessAdjustment() : base(
            hasReset: true
        )
        {
            this.Name = "AdjustBrightness";
            this.DisplayName = "Adjust brightness";
            this.GroupName = "Adjustments";

            this.ActionEditor.AddControlEx(new ActionEditorTextbox(name: "LightIpAddress", "IP Address:"));
        }
        
        protected override Boolean ApplyAdjustment(ActionEditorActionParameters actionParameters, Int32 diff)
        {
            var lightIpAddress = actionParameters.GetString("LightIpAddress");

            if (!String.IsNullOrEmpty(lightIpAddress) && IPAddress.TryParse(lightIpAddress, out _))
            {
                this._brightnessSubject.OnNext((lightIpAddress, 25));
                this.AdjustmentValueChanged();

                return true;
            }

            return base.ApplyAdjustment(actionParameters, diff);
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            var lightIpAddress = actionParameters.GetString("LightIpAddress");

            if (!String.IsNullOrEmpty(lightIpAddress) && IPAddress.TryParse(lightIpAddress, out _))
            {
                this._brightnessSubject.OnNext((lightIpAddress, 25));
                this.AdjustmentValueChanged();

                return true;
            }

            return base.RunCommand(actionParameters);
        }

        // Returns the adjustment value that is shown next to the dial.
        // protected override String GetAdjustmentValue(ActionEditorActionParameters actionParameters) =>
        //     this._brightnessSubject.Value.ToString();

        protected override Boolean OnLoad()
        {
            base.OnLoad();

            this._brightnessSubject
                .Throttle(TimeSpan.FromMilliseconds(25))
                .DistinctUntilChanged()
                .Subscribe((value) =>
                {
                    var (lightIpAddress, brightness) = value;

                    if (String.IsNullOrEmpty(lightIpAddress) || !IPAddress.TryParse(lightIpAddress, out _))
                    {
                        return;
                    }

                    this.SetBrightness(lightIpAddress, brightness);
                });

            return true;
        }

        private void SetBrightness(String lightIpAddress, Byte brightness)
        {
            _ = Task.Run(() => ElgatoLightApiClient.SetBrightness(lightIpAddress, brightness));
        }
    }
}