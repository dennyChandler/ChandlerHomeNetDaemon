using ChandlerHome.Helpers.WLEDControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Bedroom.Lighting
{
    [NetDaemonApp(Id = "Bedroom WLEDs")]
    internal class UnderBedLight : Bedroom
    {
        public UnderBedLight(IHaContext ha) : base(ha) 
        {
            _entities ??= new Entities(ha);

            _entities.BinarySensor.BedroomMotion.StateChanges().Where(e => e.New.IsOn())
                .SubscribeAsync(async x =>
                {
                    if (_entities.Sensor.WeatherflowBrightness.State < 1000)
                    {
                        var controlData = new WledControlData();

                        await controlData.TurnOnMasterBedroomNightLight("Rainbow", 255);
                    }
                });

            _entities.BinarySensor.BedroomMotion.StateChanges().Where(e => e.New.IsOff())
                .Subscribe(x =>
                {
                    if (_entities.Light.MasterBedroomNightlight.IsOn())
                        _entities.Light.MasterBedroomNightlight.TurnOff(transition: 5);
                });

        }
    }
}
