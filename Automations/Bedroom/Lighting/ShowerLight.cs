using System.Threading;

namespace ChandlerHome.Automations.Bedroom.Lighting;

[NetDaemonApp(Id = "Bedroom Shower Light")]
internal class ShowerLight : Bedroom
{
    private Logger<ShowerLight> _logger;
    private bool showerIsOn;
    private double originalAverageHumidity;
    private double humidityWhenTurnedOff;
    public ShowerLight(IHaContext ha, ILogger<ShowerLight> logger) : base(ha)
    {
        _entities ??= new Entities(ha);
        _logger = (Logger<ShowerLight>)logger;

        var humiditySensor = _entities.Sensor.BathroomHumidity;
        var showerLight = _entities.Light.MasterBathroomShowerLight;
        var averageHumidity = _entities.Sensor.AverageBathroomHumidity;
        var showerOverride = _entities.InputBoolean.ShowerOverride;

        humiditySensor.StateChanges().Where(e => e.New.State > _entities.InputNumber.ShowerLightHumidityThreshold.State)
            .Subscribe(x =>
            {
                if (showerOverride.IsOff() && showerLight.IsOff())
                    ShowerLightRoutine(humiditySensor, averageHumidity, showerOverride, _logger);
            });

        showerIsOn = showerLight.IsOn();
    }

    private void ShowerLightRoutine(NumericSensorEntity humiditySensor, NumericSensorEntity averageHumidity, InputBooleanEntity showerOverride, Logger<ShowerLight> logger)
    {
        _entities.Light.MasterBathroomShowerLight.TurnOn(brightnessPct: 100, transition: 0);
        showerIsOn = true;
        var humidityThreshold = _entities.InputNumber.ShowerLightHumidityThreshold.State;
        var humidityAtActivation = humiditySensor.State;

        Thread.Sleep(TimeSpan.FromMinutes(5));

        while (showerIsOn)
        {
            Thread.Sleep(TimeSpan.FromMinutes(1));

            originalAverageHumidity = GetAverageHumidity();
            var currentHumidity = GetHumidityState();


            if (currentHumidity > humidityThreshold + 15)
            {
                logger.LogInformation($"Current humidity: {currentHumidity}, humidityThreshold: {humidityThreshold}, humidityAtActivation: {humidityAtActivation}. Continuing!");
                continue;
            }
            else
            {
                _entities.Light.MasterBathroomShowerLight.TurnOff(transition: 30);
                logger.LogInformation($"Low Humidity (current: {currentHumidity}, threshold: {humidityThreshold}), shut off lights.");
                showerIsOn = false;
            }
        }

        if (showerOverride.IsOff())
        {
            logger.LogInformation($"Turning off, humidity: {humidityWhenTurnedOff}");
            _entities.Light.MasterBathroomShowerLight.TurnOff(transition: 30);
            showerIsOn = false;
        }
    }

    private double GetHumidityState()
    {
        return _entities.Sensor.BathroomHumidity.State ?? 0;
    }

    private double GetAverageHumidity()
    {
        return _entities.Sensor.AverageBathroomHumidity.State ?? 0;
    }
}
