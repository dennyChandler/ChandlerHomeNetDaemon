using ChandlerHome.Automations.Livingroom.Lighting.FireplaceRoutines;

namespace ChandlerHome.Automations.Livingroom.Lighting;

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
                var weatherflowBrightness = _entities.Sensor.WeatherstationIlluminance?.State ?? 0;
                int brightnessPct = weatherflowBrightness > 1000 ? 100 : 30;

                if (DateTime.Now.Hour < 21 && DateTime.Now.Hour >= 5)
                {
                    if (_entities.Light.LivingRoomLamp.IsOff())
                        TurnOn(_entities.Light.LivingRoomLamp, brightnessPct, 3);

                    if (_entities.BinarySensor.HomeBinarySensorsIsRaining.IsOn())
                    {
                        var rainPatterns = new FireplaceLightPatterns(ha);
                        rainPatterns.SetLivingRoomLightsToRain();
                    }
                    else if (HolidayLights())
                    {
                        ExecuteHolidayLightRoutine(ha, brightnessPct);
                    }
                    else
                    {
                        TurnOn(_entities.Light.FireplaceLights);
                        TurnOn(_entities.Light.ZigbeeStickGroupsMainFloorHall);
                    }
                }
                else if (DateTime.Now.Hour >= 21 || DateTime.Now.Hour < 5)
                {
                    TurnOn(_entities.Light.MainFloorHallNorthLight, 30, 0);
                }

            });

        _entities.BinarySensor.HomeBinarySensorsIsRaining.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                if (_entities.Light.FireplaceLights.IsOn())
                {
                    var rainPatterns = new FireplaceLightPatterns(ha);
                    rainPatterns.SetLivingRoomLightsToRain();
                }
                if (DateTime.Now.Hour > 5 && DateTime.Now.Hour < 21)
                    TurnOn(_entities.Light.ZigbeeStickGroupsUpstairsColorLights, 33, 3, colorName: "dodgerblue");
            });

        _entities.BinarySensor.HomeBinarySensorsIsRaining.StateChanges().Where(e => e.New.IsOff() && _entities.Light.FireplaceLights.IsOff())
            .Subscribe(x =>
            {
                TurnOff(_entities.Light.OuterFireplaceLights, 3);
                TurnOff(_entities.Light.ZigbeeStickGroupsUpstairsColorLights, 3);
                TurnOff(_entities.Light.ZigbeeStickGroupsMainFloorHall, 3);
            });

        _entities.BinarySensor.HomeBinarySensorsIsRaining.StateChanges().Where(e => e.New.IsOff() && _entities.Light.FireplaceLights.IsOn())
            .Subscribe(x =>
            {
                if (HolidayLights())
                {
                    int brightnessPct = _entities.Sensor.WeatherstationIlluminance?.State > 1000 ? 100 : 50;
                    ExecuteHolidayLightRoutine(ha, brightnessPct: brightnessPct);
                }
            });
    }

    private void ExecuteHolidayLightRoutine(IHaContext ha, int brightnessPct)
    {
        if (_entities.Light.LivingRoomLamp.IsOff())
            TurnOn(_entities.Light.LivingRoomLamp, brightnessPct, 3);

        if (HolidayLights())
        {
            var holidayLightPatterns = new FireplaceLightPatterns(ha);
            holidayLightPatterns.SetFireplaceHolidaLights();
        }
        else if (_entities.BinarySensor.HomeBinarySensorsIsRaining.IsOn())
        {
            TurnOn(_entities.Light.OuterFireplaceLights, 100, 3, "blue");
            TurnOn(_entities.Light.InnerFireplaceLights, 100, 3, "white");
            TurnOn(_entities.Light.ZigbeeStickGroupsMainFloorHall, 100, 3, colorName: "dodgerblue");
        }
    }
}




