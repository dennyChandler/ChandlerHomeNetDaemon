using ChandlerHome.Automations;

namespace ChandlerHome.apps.HassModel.Upstairs_Hall.Lighting;

[NetDaemonApp(Id = "Upstairs Hall Lighting")]
internal class HallLights : Home
{
    public HallLights(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);

        _entities.BinarySensor.UpstairsNorthHallMotion.StateChanges().Where(e => e.New.IsOn() && _entities.BinarySensor.HomeBinarySensorsIsRaining.IsOff())
            .Subscribe(x =>
            {
                if (_entities.BinarySensor.NurseryDoor.IsOn() && _entities.BinarySensor.OctaviasBedtime.IsOff() && _entities.Schedule.OctaviasRest.IsOff()) //don't turn these on if O's door is shut
                {
                    if (_entities.Sensor.WeatherstationIlluminance.State < 7000)
                        TurnOn(_entities.Light.UpstairsHallNorthLight);
                }
            });

        _entities.BinarySensor.UpstairsSouthHallMotion.StateChanges().Where(e => e.New.IsOn() && _entities.BinarySensor.HomeBinarySensorsIsRaining.IsOff())
            .Subscribe(x =>
            {
                if (_entities.BinarySensor.NurseryDoor.IsOn() && _entities.BinarySensor.OctaviasBedtime.IsOff() && _entities.Schedule.OctaviasRest.IsOff())  //don't turn these on if O's door is shut
                {
                    if (_entities.Sensor.WeatherstationIlluminance.State < 7000)
                        TurnOn(_entities.Light.UpstairsSouthHallLight);
                }
            });

        _entities.BinarySensor.TopOfStairsMotion.StateChanges().Where(e => e.New.IsOn() && _entities.BinarySensor.HomeBinarySensorsIsRaining.IsOff())
            .Subscribe(x =>
            {
                if (_entities.BinarySensor.NurseryDoor.IsOn() && _entities.BinarySensor.OctaviasBedtime.IsOff() && _entities.Schedule.OctaviasRest.IsOff()) //don't turn these on if O's door is shut
                {
                    if (_entities.Sensor.WeatherstationIlluminance.State < 7000)
                        TurnOn(_entities.Light.TopOfStairsLightLight);
                }
            });

        _entities.BinarySensor.TopOfStairsMotion.StateChanges().Where(x => x.New.IsOn() && _entities.BinarySensor.HomeBinarySensorsIsRaining.IsOn())
            .Subscribe(x =>
            {
                if (_entities.BinarySensor.NurseryDoor.IsOn() && _entities.BinarySensor.OctaviasBedtime.IsOff() && _entities.Schedule.OctaviasRest.IsOff()) //don't turn these on if O's door is shut
                {
                    TurnOn(_entities.Light.ZigbeeStickGroupsUpstairsColorLights, 100, colorName: "dodgerblue");
                }
            });

        _entities.BinarySensor.UpstairsSouthHallMotion.StateChanges().Where(x => x.New.IsOn() && _entities.BinarySensor.HomeBinarySensorsIsRaining.IsOn())
            .Subscribe(x =>
            {
                if (_entities.BinarySensor.NurseryDoor.IsOn() && _entities.BinarySensor.OctaviasBedtime.IsOff() && _entities.Schedule.OctaviasRest.IsOff()) //don't turn these on if O's door is shut
                {
                    TurnOn(_entities.Light.ZigbeeStickGroupsUpstairsColorLights, 100, colorName: "dodgerblue");
                }
            });

        _entities.BinarySensor.UpstairsNorthHallMotion.StateChanges().Where(x => x.New.IsOn() && _entities.BinarySensor.HomeBinarySensorsIsRaining.IsOn())
            .Subscribe(x =>
            {
                if (_entities.BinarySensor.NurseryDoor.IsOn() && _entities.BinarySensor.OctaviasBedtime.IsOff() && _entities.Schedule.OctaviasRest.IsOff()) //don't turn these on if O's door is shut
                {
                    TurnOn(_entities.Light.ZigbeeStickGroupsUpstairsColorLights, 100, colorName: "dodgerblue");
                }
            });

        _entities.BinarySensor.UpstairsMotion.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                TurnOff(_entities.Light.ZigbeeStickGroupsUpstairsColorLights, 5, false);
            });

        _entities.BinarySensor.NurseryDoor.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                TurnOff(_entities.Light.ZigbeeStickGroupsUpstairsColorLights, 5, false);
            });
    }
}
