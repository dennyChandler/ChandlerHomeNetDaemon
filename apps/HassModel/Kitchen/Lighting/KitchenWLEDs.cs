using ChandlerHome.Helpers.WLEDControls;
using ChandlerHome.Helpers.WLEDControls.LightRoutines;

namespace ChandlerHome.apps.HassModel.Kitchen.Lighting
{
    [NetDaemonApp(Id = "Kitchen WLEDs")]
    internal class KitchenWLEDs : LightPatterns
    {
        private TimeSpan motionTimeout = TimeSpan.FromMinutes(10);
        public KitchenWLEDs(IHaContext ha) : base(ha)
        {
            _entities ??= new Entities(ha);

            var motionOffTime = new DateTime?();

            var kitchenMotion = _entities.BinarySensor.KitchenMotion;
            var kitchenWLEDs = _entities.Light.KitchenLeds;

            kitchenMotion.StateChanges().Where(e => e.New.IsOn())
                .SubscribeAsync(async x =>
                {
                    var weatherWarnings = _entities.Sensor.NwsAlertEvent;
                    if (weatherWarnings == null || weatherWarnings.State.Equals("none", StringComparison.OrdinalIgnoreCase))
                    {
                        var controlData = new WledControlData();

                        await controlData.TurnOnKitchenWledLight("Rainbow", 255);
                    }
                    else
                    {
                        string warningState = weatherWarnings.State ?? "";
                        switch (warningState.ToUpper())
                        {
                            case string a when a.Contains("TORNADO"):
                                TornadoWarningLights();
                                break;

                        }
                    }
                    kitchenWLEDs.TurnOn(brightnessPct: 100);
                });

            Observable.Interval(TimeSpan.FromMinutes(1))
                .Subscribe(_ =>
                {
                    if (motionOffTime != null && kitchenWLEDs.IsOn())
                    {
                        // Check if it's time to turn off the lights
                        if (kitchenWLEDs.IsOn()
                        && DateTime.Now - motionOffTime >= motionTimeout
                        && kitchenMotion.IsOff())
                        {
                            TurnOff(kitchenWLEDs, transition: 60);
                            motionOffTime = null;
                        }
                    }
                    else if (motionOffTime == null && kitchenWLEDs.IsOn())
                    {
                        motionOffTime = DateTime.Now;
                    }
                });

            kitchenMotion.StateChanges()
                .Where(e => e.New.IsOff())
                .Subscribe(x =>
                {
                    motionOffTime = DateTime.Now;
                });

            _entities.BinarySensor.WeatherflowIsRaining.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x =>
                {

                });
        }
    }
}
