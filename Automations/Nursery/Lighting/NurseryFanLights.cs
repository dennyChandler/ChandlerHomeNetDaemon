namespace ChandlerHome.Automations.Nursery.Lighting;

[NetDaemonApp(Id = "Nursery Fan Light")]
internal class NurseryFanLights : Nursery
{
    private TimeSpan motionTimeout = TimeSpan.FromMinutes(10);
    private TimeSpan nurseryNightlightTimer = TimeSpan.FromMinutes(60);
    DateTime? offTime;
    DateTime? nurseryOnTime;
    public NurseryFanLights(IHaContext ha, IScheduler scheduler) : base(ha, scheduler)
    {
        _entities ??= new Entities(ha);

        scheduler.ScheduleCron("0 0 * * *", () => _entities.Light.NurseryFanLights.TurnOff(transition: 600));

        scheduler.ScheduleCron("0 14 * * *", () => _entities.Light.NurseryFanLights.TurnOff(transition: 600));

        _entities.BinarySensor.NurseryMotion.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                if (_entities.BinarySensor.OctaviasBedtime.IsOff())
                    TurnOn(_entities.Light.NurseryFanLights, 100, 10);
                offTime = null;
            });

        _entities.BinarySensor.NurseryDoor.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                if (_entities.Light.NurseryFanLights.IsOn())
                    TurnOn(_entities.Light.NurseryFanLights, transition: 600, brightnessPercent: 10);
                offTime = null;
            });

        _entities.BinarySensor.NurseryMotion.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                offTime = DateTime.Now;
            });

        Observable.Interval(TimeSpan.FromMinutes(1))
            .Subscribe(_ =>
            {
                if (_entities.BinarySensor.NurseryDoor.IsOn())
                {
                    if (offTime != null)
                    {
                        // Check if it's time to turn off the lights
                        if (_entities.Light.NurseryFanLights.IsOn()
                        && DateTime.Now - offTime >= motionTimeout
                        && _entities.BinarySensor.NurseryMotion.IsOff())
                        {
                            TurnOff(_entities.Light.NurseryFanLights, transition: 60);
                            offTime = null;
                        }
                    }
                    else if (offTime == null && _entities.BinarySensor.NurseryMotion.IsOff() && _entities.Light.NurseryFanLights.IsOn())
                    {
                        offTime = DateTime.Now;
                    }
                }
                if (_entities.BinarySensor.NurseryDoor.IsOff())
                {
                    if (nurseryOnTime != null)
                    {
                        if (DateTime.Now - nurseryOnTime >= nurseryNightlightTimer)
                        {
                            TurnOff(_entities.Light.NurseryFanLights, transition: 60);
                            nurseryOnTime = null;
                            _entities.InputBoolean.NurseryNightlightButtonPress.TurnOff();
                        }
                    }
                }

                if (_entities.BinarySensor.OctaviasBedtime.IsOn() || _entities.Schedule.OctaviasRest.IsOn())
                {
                    if (_entities.Light.NurseryFanLights.IsOn() && DateTime.Now - offTime >= motionTimeout)
                    {
                        TurnOff(_entities.Light.NurseryFanLights, transition: 60);
                    }
                }
            });
    }
}
