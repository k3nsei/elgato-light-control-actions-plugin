namespace Loupedeck.ElgatoLightControlPlugin
{
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Constants;

    using ElgatoLightApiClient;

    public class BrightnessAdjustment : PluginDynamicAdjustment
    {
        private String _currentLightIpAddress = "";
        private readonly BehaviorSubject<Byte> _brightnessSubject = new(25);

        public BrightnessAdjustment() : base(hasReset: true)
        {
            this.Name = "AdjustBrightness";
            this.DisplayName = "Adjust brightness";
            this.Description = "Adjust brightness of the light";
            this.GroupName = ActionGroupName.Adjustments;

            this.AdjustmentMinValue = 1;
            this.AdjustmentMaxValue = 100;
            this.AdjustmentDefaultValue = 25;

            this.MakeProfileAction("text;IP Address:");
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            PluginLog.Info($"AA ap {actionParameter ?? "none"}");

            var lightIpAddress = actionParameter;

            if (String.IsNullOrEmpty(lightIpAddress) || !IPAddress.TryParse(lightIpAddress, out _))
            {
                this._currentLightIpAddress = String.Empty;

                return;
            }

            var brightness = (Byte)Math.Clamp(this._brightnessSubject.Value + diff, 1, 100);

            this._currentLightIpAddress = lightIpAddress;
            this._brightnessSubject.OnNext(brightness);

            this.AdjustmentValueChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            PluginLog.Info($"RC ap {actionParameter ?? "none"}");

            var lightIpAddress = actionParameter;

            if (String.IsNullOrEmpty(lightIpAddress) || !IPAddress.TryParse(lightIpAddress, out _))
            {
                this._currentLightIpAddress = String.Empty;

                return;
            }

            this._currentLightIpAddress = lightIpAddress;
            this._brightnessSubject.OnNext(25);

            this.AdjustmentValueChanged();
        }

        // Returns the adjustment value that is shown next to the dial.
        protected override String GetAdjustmentValue(String actionParameters) =>
            this._brightnessSubject.Value.ToString();

        protected override Boolean OnLoad()
        {
            base.OnLoad();

            this._brightnessSubject
                .Throttle(TimeSpan.FromMilliseconds(25))
                .DistinctUntilChanged()
                .Subscribe((brightness) =>
                {
                    var lightIpAddress = this._currentLightIpAddress;

                    if (String.IsNullOrEmpty(lightIpAddress))
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