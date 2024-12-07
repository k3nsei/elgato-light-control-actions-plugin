namespace Loupedeck.ElgatoLightControlPlugin
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using ElgatoLightControl.ApiClient;

    public class ColorTemperatureAdjustment() : PluginDynamicAdjustment(
        displayName: "Adjust color temperature",
        description: "Adjust light color temperature",
        groupName: "Adjustments",
        hasReset: true
    )
    {
        private readonly BehaviorSubject<UInt16> _colorTemperatureSubject = new(143);

        // This method is called when the adjustment is executed.
        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            var colorTemperature = (UInt16)Math.Clamp(this._colorTemperatureSubject.Value + diff, 143, 344);

            this._colorTemperatureSubject.OnNext(colorTemperature);
            this.AdjustmentValueChanged();
        }

        // This method is called when the reset command related to the adjustment is executed.
        protected override void RunCommand(String actionParameter)
        {
            this._colorTemperatureSubject.OnNext(143);
            this.AdjustmentValueChanged();
        }

        // Returns the adjustment value that is shown next to the dial.
        protected override String GetAdjustmentValue(String actionParameter) =>
            this._colorTemperatureSubject.Value.ToString();

        protected override Boolean OnLoad()
        {
            base.OnLoad();

            // this._colorTemperatureSubject
            //     .Throttle(TimeSpan.FromMilliseconds(25))
            //     .DistinctUntilChanged()
            //     .Subscribe(this.SetColorTemperature);

            return true;
        }

        private void SetColorTemperature(UInt16 colorTemperature)
        {
            const String lightIpAddress = "192.168.0.11";

            _ = Task.Run(() => ApiClient.SetColorTemperature(lightIpAddress, colorTemperature));
        }
    }
}