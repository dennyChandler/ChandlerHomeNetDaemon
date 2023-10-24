namespace ChandlerHome.Automations.BrittanysOffice.Lighting;

[NetDaemonApp(Id = "Brittany Fan Lights")]
internal class BrittanyFanLights : BrittanysOffice
{
    private TimeSpan motionTimeout = TimeSpan.FromMinutes(10);
    DateTime? offTime;
    public BrittanyFanLights(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);

        _entities.BinarySensor.BrittanysOfficeMotion.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                TurnOn(_entities.Light.BrittanysOfficeLights, 100, 10);
            });

        _entities.BinarySensor.BrittanysOfficeMotion.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                offTime = DateTime.Now;
            });

        Observable.Interval(TimeSpan.FromMinutes(1))
            .Subscribe(_ =>
            {
                if (offTime != null)
                {
                    // Check if it's time to turn off the lights
                    if (_entities.Light.BrittanysOfficeLights.IsOn()
                    && DateTime.Now - offTime >= motionTimeout
                    && _entities.BinarySensor.BrittanysOfficeMotion.IsOff())
                    {
                        TurnOff(_entities.Light.BrittanysOfficeLights, transition: 60);
                        offTime = null;
                    }
                }
                else if (offTime == null && _entities.BinarySensor.BrittanysOfficeMotion.IsOff() && _entities.Light.BrittanysOfficeLights.IsOn())
                {
                    offTime = DateTime.Now;
                }
            });

    }
}
