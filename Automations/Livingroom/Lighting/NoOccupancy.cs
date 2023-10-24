namespace ChandlerHome.Automations.Livingroom.Lighting;

[NetDaemonApp(Id = "Living Room No Occupancy")]
public class NoOccupancy : Livingroom
{
    private TimeSpan motionTimeout = TimeSpan.FromMinutes(10);
    private TimeSpan nightMotionTimeout = TimeSpan.FromMinutes(5);
    public NoOccupancy(IHaContext ha) : base(ha)
    {
        if (_entities == null)
            _entities = new Entities(ha);

        var motionOffTime = new DateTime?();

        _entities.BinarySensor.LivingRoomMotion
            .StateChanges()
            .Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                motionOffTime = DateTime.Now;
            });

        _entities.BinarySensor.LivingRoomMotion
            .StateChanges()
            .Where(e => e.New.IsOff()
                && _entities.MediaPlayer.Shield.State.Equals("off", StringComparison.OrdinalIgnoreCase)
                && _entities.Light.LivingRoomLamp.IsOn() && _entities.BinarySensor.LivingRoomOccupancy.IsOff())
            .Subscribe(x =>
            {
                TurnOff(_entities.Light.LivingRoomLamp, transition: 3);
                TurnOff(_entities.Light.FireplaceLights, transition: 60);
                TurnOff(_entities.Light.ZigbeeStickGroupsMainFloorHall, transition: 10);
                TurnOff(_entities.Light.LivingRoomFan, transition: 10);
            });

        Observable.Interval(TimeSpan.FromMinutes(3))
            .Subscribe(_ =>
            {
                if (motionOffTime != null)
                {
                    if (DateTime.Now.Hour > 5 && DateTime.Now.Hour < 20)
                    {
                        // Check if it's time to turn off the lights
                        if ((_entities.Light.LivingRoomLamp.IsOn() || _entities.Light.ZigbeeStickGroupsMainFloorHall.IsOn())
                        && DateTime.Now - motionOffTime >= motionTimeout
                        && _entities.MediaPlayer.Shield.State.Equals("off", StringComparison.OrdinalIgnoreCase)
                        && _entities.BinarySensor.LivingRoomMotion.IsOff())
                        {
                            TurnOff(_entities.Light.LivingRoomLamp, transition: 3);
                            TurnOff(_entities.Light.FireplaceLights, transition: 60);
                            TurnOff(_entities.Light.ZigbeeStickGroupsMainFloorHall, transition: 10);
                            motionOffTime = null;
                        }
                        else if (DateTime.Now - motionOffTime >= motionTimeout) //stop it from killing everything as soon as it observes the shield being stopped for 5 minutes
                        {
                            motionOffTime = DateTime.Now;
                        }
                    }
                    else
                    {
                        // Check if it's time to turn off the lights
                        if ((_entities.Light.LivingRoomLamp.IsOn() || _entities.Light.ZigbeeStickGroupsMainFloorHall.IsOn())
                        && DateTime.Now - motionOffTime >= nightMotionTimeout
                        && _entities.MediaPlayer.Shield.State.Equals("off", StringComparison.OrdinalIgnoreCase)
                        && _entities.BinarySensor.LivingRoomMotion.IsOff())
                        {
                            TurnOff(_entities.Light.LivingRoomLamp, transition: 3);
                            TurnOff(_entities.Light.FireplaceLights, transition: 60);
                            TurnOff(_entities.Light.ZigbeeStickGroupsMainFloorHall, transition: 15);
                            motionOffTime = null;
                        }
                        else if (DateTime.Now - motionOffTime >= nightMotionTimeout) //stop it from killing everything as soon as it observes the shield being stopped for 5 minutes
                        {
                            motionOffTime = DateTime.Now;
                        }
                    }
                }
                if (motionOffTime == null && (_entities.Light.LivingRoomLamp.IsOn() || _entities.Light.ZigbeeStickGroupsMainFloorHall.IsOn() || _entities.Light.FireplaceLights.IsOn())
                && _entities.BinarySensor.LivingRoomMotion.IsOff())
                    motionOffTime = DateTime.Now;
            });
    }
}
