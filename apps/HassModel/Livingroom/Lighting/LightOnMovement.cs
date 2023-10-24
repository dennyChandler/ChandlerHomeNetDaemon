using ChandlerHome.apps.HassModel.Livingroom.Lighting.FireplaceRoutines;
using HomeAssistantGenerated;

namespace ChandlerHome.apps.HassModel.Livingroom
{
    [NetDaemonApp(Id = "Living Room Lights")]
    public class LightOnMovement : Livingroom
    {
        public LightOnMovement(IHaContext ha) : base(ha)
        {
            if (_entities == null)
                _entities = new Entities(ha);

            var livingRoomMotion = _entities.BinarySensor.LivingRoomMotion;

            livingRoomMotion
                .StateChanges()
                .Where(e => e.New.IsOn())
                .Subscribe(x =>
                {
                    var weatherflowBrightness = _entities.Sensor.WeatherflowBrightness?.State ?? 0;
                    int brightnessPct = weatherflowBrightness > 4000 ? 100 : 50;

                    if (_entities.Light.LivingRoomLamp.IsOff())
                        TurnOn(_entities.Light.LivingRoomLamp, brightnessPct, 3);

                    if (HolidayLights())
                    {
                        var holidayLightPatterns = new FireplaceLightPatterns(ha);
                        holidayLightPatterns.SetFireplaceHolidaLights();
                    }
                    else if (_entities.BinarySensor.WeatherflowIsRaining.IsOn())
                    {
                        TurnOn(_entities.Light.OuterFireplaceLights, 100, 3, "blue");
                        TurnOn(_entities.Light.InnerFireplaceLights, 100, 3, "white");
                    }
                    else
                        TurnOn(_entities.Light.FireplaceLights, 100, 3, "white");
                });

            _entities.BinarySensor.WeatherflowIsRaining.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x =>
                {
                    if (_entities.Light.FireplaceLights.IsOn())
                        TurnOn(_entities.Light.OuterFireplaceLights, 100, 3, "blue");
                });

            _entities.BinarySensor.WeatherflowIsRaining.StateChanges().Where(e => e.New.IsOff() && _entities.Light.FireplaceLights.IsOn())
                .Subscribe(x =>
                {
                    TurnOff(_entities.Light.OuterFireplaceLights, 3);
                });
        }



        
    }
}
