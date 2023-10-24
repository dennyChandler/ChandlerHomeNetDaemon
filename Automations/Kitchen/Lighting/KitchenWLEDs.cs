using ChandlerHome.Helpers.WLEDControls;
using ChandlerHome.Helpers.WLEDControls.LightRoutines;

namespace ChandlerHome.Automations.Kitchen.Lighting;

[NetDaemonApp(Id = "Kitchen WLEDs")]
internal class KitchenWLEDs : KitchenLightPatterns
{
    private TimeSpan motionTimeout = TimeSpan.FromMinutes(3);
    private ILogger<KitchenWLEDs> _logger;
    public KitchenWLEDs(IHaContext ha, ILogger<KitchenWLEDs> logger) : base(ha)
    {
        _logger = logger;
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
                    if (_entities.BinarySensor.HomeBinarySensorsIsRaining.IsOn())
                    {
                        var controlData = new WledControlData("Rain", 45, 175, 255, palette: 7);
                        _logger.LogDebug("Rain, starting Rain");
                        await controlData.TurnOnKitchenWledLight();
                    }
                    else
                    {
                        var controlData = new WledControlData("Rainbow", 30, 100, 255);
                        _logger.LogDebug("No Weather Warning, starting Rainbow");
                        await controlData.TurnOnKitchenWledLight();
                    }
                }
                else
                {
                    string warningState = weatherWarnings.State ?? "";
                    switch (warningState.ToUpper())
                    {
                        case string a when a.Contains("TORNADO", StringComparison.OrdinalIgnoreCase):
                            _logger.LogDebug("Tornado Warning!");
                            TornadoWarningLights();
                            break;
                        case string a when a.Contains("Freeze", StringComparison.OrdinalIgnoreCase):
                            _logger.LogDebug("Freeze Warning!");
                            FreezeWarningLights();
                            break;
                        case string a when a.Contains("Winter", StringComparison.OrdinalIgnoreCase):
                            _logger.LogDebug("Winter Warning!");
                            WinterWarningLights();
                            break;
                        case string a when a.Contains("Wind", StringComparison.OrdinalIgnoreCase):
                            _logger.LogDebug("Wind Warning!");
                            WindWarningLights();
                            break;
                        case string a when a.Contains("Thunderstorm", StringComparison.OrdinalIgnoreCase):
                            _logger.LogDebug("Thunderstorm Warning!");
                            ThunderstormWarningLights();
                            break;
                        case string a when a.Contains("Flood", StringComparison.OrdinalIgnoreCase):
                            _logger.LogDebug("Flood Warning!");
                            FloodWarningLights();
                            break;
                        case string a when a.Contains("Heat", StringComparison.OrdinalIgnoreCase):
                            _logger.LogDebug("Heat Warning!");
                            HeatWarningLights();
                            break;
                        case string a when a.Contains("Flag", StringComparison.OrdinalIgnoreCase):
                            _logger.LogDebug("Flag Warning!");
                            RedFlagWarningLights();
                            break;
                        default:
                            var controlData = new WledControlData("Rainbow", 30, 100, 255);
                            _logger.LogDebug("Default Light With Warning!");
                            await controlData.TurnOnKitchenWledLight();
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

        _entities.BinarySensor.HomeBinarySensorsIsRaining.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                RainKitchenLights();
            });
    }
}



