using ChandlerHome.Automations;

namespace ChandlerHome.Automations.Entryway.Lighting;

[NetDaemonApp(Id = "Entryway Lights")]
internal class EntrywayLights : Home
{
    DateTime? offTime;
    TimeSpan motionTimeout = TimeSpan.FromMinutes(5);
    public EntrywayLights(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);

        var entrywayMotion = _entities.BinarySensor.EntrywayMotion;
        var entrywayLights = _entities.Light.ZgroupEntrywayLights;


        entrywayMotion.StateChanges().Where(e => e.New.IsOn() && DateTime.Now.Hour < 21 && DateTime.Now.Hour > 7)
            .Subscribe(x =>
            {
                if (entrywayLights.IsOff() && DateTime.Now.Hour > 8 && DateTime.Now.Hour < 21 ||
                _entities.BinarySensor.Pixel8aIsCharging.IsOff() && _entities.BinarySensor.BrittanysPhoneIsCharging.IsOff())
                    TurnOn(entrywayLights, 100, 0);
                else if (_entities.BinarySensor.Pixel8aIsCharging.IsOn() || _entities.BinarySensor.BrittanysPhoneIsCharging.IsOn()
                && _entities.Sensor.WeatherstationIlluminance.State < 2000)
                    TurnOn(entrywayLights, 10, 0);
            });

        entrywayMotion.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                offTime = DateTime.Now;

            });

        Observable.Interval(TimeSpan.FromMinutes(1))
        .Subscribe(_ =>
        {
            if (offTime != null)
            {
                var timeDiff = DateTime.Now - offTime;
                // Check if it's time to turn off the lights
                if (_entities.Light.ZgroupEntrywayLights.IsOn()
                && timeDiff >= motionTimeout
                && _entities.BinarySensor.EntrywayMotion.IsOff())
                {
                    TurnOff(entrywayLights, 3, false);
                    offTime = null;
                }
            }
            else if (offTime == null && _entities.BinarySensor.EntrywayMotion.IsOff() && _entities.Light.ZgroupEntrywayLights.IsOn())
            {
                offTime = DateTime.Now;
            }
        });
    }
}
