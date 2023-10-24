using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Bedroom.Lighting
{
    [NetDaemonApp(Id = "Bedroom Shower Light")]
    internal class ShowerLight : Bedroom
    {
        private Logger<ShowerLight> _logger;
        private bool showerIsOn;
        private double originalAverageHumidity;
        private double humidityWhenTurnedOff;
        public ShowerLight(IHaContext ha, ILogger<ShowerLight> logger):base(ha)
        {
            _entities ??= new Entities(ha);
            _logger = (Logger<ShowerLight>)logger;

            var humiditySensor = _entities.Sensor.BathroomHumidity;
            var showerLight = _entities.Light.MasterBathroomShowerLight;
            var averageHumidity = _entities.Sensor.AverageBathroomHumidity;
            var showerOverride = _entities.InputBoolean.ShowerOverride;

            humiditySensor.StateChanges().Where(e => e.New.State > averageHumidity.State + 10)
                .Subscribe(x =>
                {
                    if (showerOverride.IsOff() && showerLight.IsOff())
                        ShowerLightRoutine(humiditySensor, averageHumidity, showerOverride, _logger);
                });

            showerIsOn = humiditySensor.State.Value > averageHumidity.State + 10 && showerLight.IsOn();
        }

        private void ShowerLightRoutine(NumericSensorEntity humiditySensor, NumericSensorEntity averageHumidity, InputBooleanEntity showerOverride, Logger<ShowerLight> logger)
        {
            _entities.Light.MasterBathroomShowerLight.TurnOn(brightnessPct: 100, transition: 0);
            showerIsOn = true;
            var previousHumidity = GetHumidityState();
            var outdoorTemp = _entities.Sensor.WeatherflowAirTemperature.State ?? 0;
            var humidityThreshold = GetHumidityThreshold(outdoorTemp);

            Thread.Sleep(TimeSpan.FromMinutes(5));

            while (showerIsOn)
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));

                originalAverageHumidity = GetAverageHumidity();
                var currentHumidity = GetAverageHumidity();


                if (currentHumidity > humidityThreshold)
                {
                    logger.LogInformation($"Current humidity: {currentHumidity}, humidityThreshold: {humidityThreshold}, continuing!");
                    continue;
                }
                if (currentHumidity - originalAverageHumidity < 7)
                {
                    // Humidity decreased by 7 or more, exit the loop
                    humidityWhenTurnedOff = GetHumidityState();
                    showerIsOn = false;
                    logger.LogInformation($"Current humidity - originalAverageHumidity < 7: {currentHumidity - originalAverageHumidity}");
                }
                else if (currentHumidity < 75) //humiditiy is below initial cause for loop, turn off
                {
                    humidityWhenTurnedOff = GetHumidityState();
                    showerIsOn = false;
                    logger.LogInformation($"Current humidity < 75: {currentHumidity}");
                }
                else if (currentHumidity - previousHumidity < -8)// change is greater than 5% decrease in humidity in 1 minute, shower is off, exit loop
                {
                    humidityWhenTurnedOff = GetHumidityState();
                    showerIsOn = false;
                    logger.LogInformation($"Current humidity - previousHumidity < -8: {currentHumidity - previousHumidity}");
                }
                else
                {
                    previousHumidity = currentHumidity;
                    logger.LogInformation($"all good, cycle again.");
                }                
            }

            if (showerOverride.IsOff())
            {
                logger.LogInformation($"Turning off, humidity: {humidityWhenTurnedOff}");
                _entities.Light.MasterBathroomShowerLight.TurnOff(transition: 120);
            }
        }

        private double GetHumidityThreshold(double outdoorTemp)
        {
            if (outdoorTemp == 0)
                return 75;
            else if (outdoorTemp > 70)
                return 75;
            else if (outdoorTemp > 50 && outdoorTemp < 70)
                return 60;
            else if (outdoorTemp < 50)
                return 50;
            else return 75;
        }

        private double GetHumidityState()
        {
            return (_entities.Sensor.BathroomHumidity.State ?? 0);
        }

        private double GetAverageHumidity()
        {
            return _entities.Sensor.AverageBathroomHumidity.State ?? 0;
        }
    }
}
