namespace ChandlerHome.Automations.Bedroom.Lighting;

[NetDaemonApp(Id = "Bedroom Closet Light")]
internal class ClosetLight : Bedroom
{
    private TimeSpan motionTimeout = TimeSpan.FromMinutes(5);
    DateTime? offTime;
    public ClosetLight(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);

        _entities.BinarySensor.ClosetMotionMotion.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                _entities.Light.BedroomClosetLight.TurnOn(brightnessPct: 100, rgbwwColor: new int[] { 0, 0, 0, 255, 0 });
            });

        _entities.BinarySensor.ClosetMotionMotion.StateChanges().Where(e => e.New.IsOff())
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
               if (_entities.Light.BedroomClosetLight.IsOn()
                && DateTime.Now - offTime >= motionTimeout
                && (_entities.BinarySensor.ClosetMotionMotion.IsOff() || _entities.BinarySensor.ClosetMotionMotion.State.Equals("unknown", StringComparison.OrdinalIgnoreCase)))
               {
                   TurnLightOff(_entities.Light.BedroomClosetLight, 0, false);
                   offTime = null;
               }
           }
           else if (offTime == null && (_entities.BinarySensor.ClosetMotionMotion.IsOff() || _entities.BinarySensor.ClosetMotionMotion.State.Equals("unknown", StringComparison.OrdinalIgnoreCase)) && _entities.Light.BedroomClosetLight.IsOn())
           {
               offTime = DateTime.Now;
           }
       });
    }
}
