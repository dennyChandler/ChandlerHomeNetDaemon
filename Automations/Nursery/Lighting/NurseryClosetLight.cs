namespace ChandlerHome.Automations.Nursery.Lighting;

[NetDaemonApp(Id = "Nursery Closet Light")]
internal class NurseryClosetLight : Nursery
{
    public NurseryClosetLight(IHaContext ha, IScheduler scheduler) : base(ha, scheduler)
    {
        _entities ??= new Entities(ha);

        _entities.BinarySensor.NurseryClosetDoor.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                TurnOff(_entities.Light.NurseryClosetLight, 10);
            });

        _entities.BinarySensor.NurseryClosetDoor.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                TurnOn(_entities.Light.NurseryClosetLight, 100, 3, rgbwwValue: new int[] { 0, 0, 0, 0, 255 });
            });
    }
}
