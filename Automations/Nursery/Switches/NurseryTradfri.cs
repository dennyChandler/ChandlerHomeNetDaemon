namespace ChandlerHome.Automations.Nursery.Switches;

[NetDaemonApp(Id = "Nursery Tradfri")]
internal class NurseryTradfri : Nursery
{
    private DateTime? motionOff;
    private TimeSpan motionTimeout = TimeSpan.FromMinutes(10);
    public NurseryTradfri(IHaContext ha, IScheduler scheduler) : base(ha, scheduler)
    {
        _entities ??= new Entities(ha);

        _entities.BinarySensor.NurseryMotion.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                _entities.Switch.NurseryTradfriSwitch.TurnOn();
            });

        _entities.BinarySensor.NurseryMotion.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                motionOff = DateTime.Now;
            });

        Observable.Interval(TimeSpan.FromMinutes(1))
            .Subscribe(_ =>
            {
                if (motionOff != null)
                {
                    // Check if it's time to turn off the lights
                    if (_entities.Switch.NurseryTradfriSwitch.IsOn()
                    && DateTime.Now - motionOff >= motionTimeout
                    && _entities.BinarySensor.NurseryMotion.IsOff()
                    && _entities.BinarySensor.NurseryDoor.IsOn())
                    {
                        _entities.Switch.NurseryTradfriSwitch.TurnOff();
                        motionOff = null;
                    }
                }
                else if (motionOff == null && _entities.BinarySensor.NurseryMotion.IsOff() && _entities.Switch.NurseryTradfriSwitch.IsOn())
                {
                    motionOff = DateTime.Now;
                }
            });

    }
}
