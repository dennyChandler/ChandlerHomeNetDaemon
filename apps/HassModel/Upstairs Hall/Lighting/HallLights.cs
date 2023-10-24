using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Upstairs_Hall.Lighting
{
    [NetDaemonApp(Id = "Upstairs Hall Lighting")]
    internal class HallLights : Home
    {
        public HallLights(IHaContext ha) : base(ha)
        {
            _entities ??= new Entities(ha);

            _entities.BinarySensor.UpstairsNorthHallMotion.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x =>
                {
                    if (_entities.BinarySensor.NurseryDoor.IsOn()) //don't turn these on if O's door is shut
                    {
                        if (_entities.Sensor.WeatherflowBrightness.State < 5000 || _entities.Sensor.NorthHallLux.State < 5)
                            TurnOn(_entities.Light.UpstairsNorthLight, 100, 3);
                    }
                });

            _entities.BinarySensor.UpstairsSouthHallMotion.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x =>
                {
                    if (_entities.BinarySensor.NurseryDoor.IsOn()) //don't turn these on if O's door is shut
                    {
                        if (_entities.Sensor.WeatherflowBrightness.State < 5000 || _entities.Sensor.NorthHallLux.State < 5)
                            TurnOn(_entities.Light.UpstairsSouthLight, 100, 3);
                    }
                });

            _entities.BinarySensor.TopOfStairsMotion.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x =>
                {
                    if (_entities.BinarySensor.NurseryDoor.IsOn()) //don't turn these on if O's door is shut
                    {
                        if (_entities.Sensor.WeatherflowBrightness.State < 5000 || _entities.Sensor.NorthHallLux.State < 5)
                            TurnOn(_entities.Light.TopOfStairsLight, 100, 3);
                    }
                });

            _entities.BinarySensor.UpstairsMotion.StateChanges().Where(e => e.New.IsOff())
                .Subscribe(x =>
                {
                    TurnOff(_entities.Light.UpstairsHallLights, 5, false);
                });

            _entities.BinarySensor.NurseryDoor.StateChanges().Where(e => e.New.IsOff())
                .Subscribe(x =>
                {
                    TurnOff(_entities.Light.UpstairsHallLights, 5, false);
                });
        }
    }
}
