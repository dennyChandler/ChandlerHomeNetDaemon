using ChandlerHome.Helpers.WLEDControls;

namespace ChandlerHome.apps.HassModel.Nursery.Lighting;

[NetDaemonApp(Id = "Nursery Nightlight")]
internal class NurseryNightlight : Nursery
{
    private DateTime? motionOff;
    private TimeSpan motionTimeout = TimeSpan.FromMinutes(10);

    public NurseryNightlight(IHaContext ha, IScheduler scheduler) : base(ha, scheduler)
    {
        _entities ??= new Entities(ha);

        _entities.BinarySensor.NurseryMotion.StateChanges().Where(e => e.New.IsOn())
            .SubscribeAsync(async x =>
            {
                if (_entities.Light.OctaviaSNightlight.IsOff())
                {
                    var controlData = new WledControlData("Solid", 25, 100, 10);
                    _entities.Light.OctaviaSNightlight.TurnOn(colorName: "white");

                    await controlData.TurnOnNurseryNightLight();
                }
            });

        _entities.BinarySensor.NurseryMotion.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                motionOff = DateTime.Now;
            });

        Observable.Interval(TimeSpan.FromMinutes(5))
            .Subscribe(_ =>
            {
                if (motionOff != null)
                {
                    // Check if it's time to turn off the lights
                    if (_entities.Light.OctaviaSNightlight.IsOn()
                    && DateTime.Now - motionOff >= motionTimeout
                    && _entities.BinarySensor.NurseryMotion.IsOff()
                    && _entities.BinarySensor.NurseryDoor.IsOn())
                    {
                        TurnOff(_entities.Light.OctaviaSNightlight, transition: 60);
                        motionOff = null;
                    }
                }
                else if (motionOff == null && _entities.BinarySensor.NurseryMotion.IsOff() && _entities.BinarySensor.NurseryDoor.IsOn() && _entities.Light.OctaviaSNightlight.IsOn())
                {
                    motionOff = DateTime.Now;
                }
            });

        _entities.BinarySensor.NurseryDoor.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                motionOff = null;
            });
    }
}
