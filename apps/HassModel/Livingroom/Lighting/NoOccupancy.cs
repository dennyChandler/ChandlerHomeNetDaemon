using System.Reactive.Linq;

namespace ChandlerHome.apps.HassModel.Livingroom
{
    [NetDaemonApp(Id = "Living Room No Occupancy")]
    public class NoOccupancy : Livingroom
    {
        private TimeSpan motionTimeout = TimeSpan.FromMinutes(10);
        public NoOccupancy(IHaContext ha) : base(ha)
        {
            if (_entities == null)
                _entities = new Entities(ha);

            var livingRoomMotion = _entities.BinarySensor.LivingRoomMotion;

            var motionOffTime = new DateTime?();

            livingRoomMotion
                .StateChanges()
                .Where(e => e.New.IsOff())
                .Subscribe(x =>
                {
                    motionOffTime = DateTime.Now;
                });

            livingRoomMotion
                .StateChanges()
                .Where(e => e.New.IsOff()
                    && (!_entities.MediaPlayer.Shield.State.Equals("playing", StringComparison.OrdinalIgnoreCase) ||
                     (!_entities.MediaPlayer.Shield.State.Equals("paused", StringComparison.OrdinalIgnoreCase)))
                    && _entities.BinarySensor.LivingRoomOccupancy.IsOff()
                    && _entities.Light.LivingRoomLamp.IsOn())
                .Subscribe(x =>
                {
                    TurnOff(_entities.Light.LivingRoomLamp, transition: 3);
                    TurnOff(_entities.Light.FireplaceLights, transition: 60);
                });

            Observable.Interval(TimeSpan.FromMinutes(1))
                .Subscribe(_ =>
                {
                    if (motionOffTime != null)
                    {
                        // Check if it's time to turn off the lights
                        if (_entities.Light.LivingRoomLamp.IsOn()
                        && DateTime.Now - motionOffTime >= motionTimeout
                        && !_entities.MediaPlayer.Shield.State.Equals("playing", StringComparison.OrdinalIgnoreCase)
                        && livingRoomMotion.IsOff())
                        {
                            TurnOff(_entities.Light.LivingRoomLamp, transition: 3);
                            TurnOff(_entities.Light.FireplaceLights, transition: 60);
                            motionOffTime = null;
                        }
                        else if (DateTime.Now - motionOffTime >= motionTimeout) //stop it from killing everything as soon as it observes the shield being stopped for 5 minutes
                        {
                            motionOffTime = DateTime.Now;
                        }
                    }
                });
        }
    }
}
